using AspNet.Security.OAuth.Strava;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Repositories;
using OBPostupyApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class StravaService : IStravaService
    {
        private readonly HttpClient _httpClient;
        private readonly StravaSettings _settings;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StravaService> _logger;
        private const string ACCESS_TOKEN = "access_token";
        private const string REFRESH_TOKEN = "refresh_token";
        private const string EXPIRES_AT = "expires_at";
        private const string STRAVA = "Strava";

        public StravaService(HttpClient httpClient, IOptions<StravaSettings> settings, UserManager<User> userManager, IUserRepository userRepository, ILogger<StravaService> logger)
        {
            _httpClient = httpClient;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<StravaActivitiesResponse> GetActivityListAsync(string date, ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            var accessToken = await GetTokenAsync(user);
            if (user == null || accessToken == null)
            {
                return new StravaActivitiesResponse { ResponseType = ResponseType.Unauthorization };
            }

            var dateParameter = GetDateParameters(date);
            var response = await _httpClient.GetAsync($"/api/v3/athlete/activities?{dateParameter}access_token={accessToken}");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var activites = JsonConvert.DeserializeObject<List<SummaryActivity>>(responseBody);
                return new StravaActivitiesResponse
                {
                    ResponseType = ResponseType.OK,
                    Activities = activites.Select(a => new StravaActivityResponse
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Distance = GetDistanceString(a.Distance),
                        Time = TimeSpan.FromSeconds(a.MovingTime)
                    }).ToList()
                };
            }
            return new StravaActivitiesResponse { ResponseType = ResponseType.BadRequest };
        }

        public async Task<PathDataResponse> GetActivityAsync(long activityId, ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            var accessToken = await GetTokenAsync(user);
            if (user == null || accessToken == null)
            {
                return new PathDataResponse { ResponseType = ResponseType.Unauthorization };
            }

            var response = await _httpClient.GetAsync($"/api/v3/activities/{activityId}/streams?keys=latlng,time&key_by_type=true&access_token={accessToken}");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var streamSet = JsonConvert.DeserializeObject<StreamSet>(responseBody);
                var pathData = new List<PathData>();
                int minCount = Math.Min(streamSet.Latlng.Data.Count, streamSet.Time.Data.Count);
                var start = DateTime.MinValue;
                for (int i = 0; i < minCount; i++)
                {
                    pathData.Add(new PathData
                    {
                        Lat = streamSet.Latlng.Data[i][0],
                        Lon = streamSet.Latlng.Data[i][1],
                        Timestamp = start.AddSeconds(streamSet.Time.Data[i])
                    });
                }

                return new PathDataResponse
                {
                    ResponseType = ResponseType.OK,
                    Locations = pathData
                };
            }

            return new PathDataResponse { ResponseType = ResponseType.BadRequest };
        }

        public async Task<ResponseType> SetAuthTokensAsync(string userId, IEnumerable<AuthenticationToken> tokens)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ResponseType.Unauthorization;
            }

            var accessToken = tokens.Single(f => f.Name == "access_token").Value;
            var refreshToken = tokens.Single(f => f.Name == "refresh_token").Value;
            var expiryDate = tokens.Single(f => f.Name == "expires_at").Value;
            await _userManager.SetAuthenticationTokenAsync(user, StravaAuthenticationDefaults.AuthenticationScheme, ACCESS_TOKEN, accessToken);
            await _userManager.SetAuthenticationTokenAsync(user, StravaAuthenticationDefaults.AuthenticationScheme, REFRESH_TOKEN, refreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, StravaAuthenticationDefaults.AuthenticationScheme, EXPIRES_AT, expiryDate);

            return ResponseType.OK;
        }

        private async Task<string> GetTokenAsync(User user)
        {
            var accessToken = await _userManager.GetAuthenticationTokenAsync(user, STRAVA, ACCESS_TOKEN);
            var expiresToken = await _userManager.GetAuthenticationTokenAsync(user, STRAVA, EXPIRES_AT);
            var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, STRAVA, REFRESH_TOKEN);

            if (accessToken == null)
            {
                return null;
            }

            if (expiresToken != null && DateTime.Parse(expiresToken) < DateTime.Now)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("client_id", _settings.ClientId);
                parameters.Add("client_secret", _settings.ClientSecret);
                parameters.Add("grant_type", "refresh_token");
                parameters.Add("refresh_token", refreshToken);
                var parametersContent = new FormUrlEncodedContent(parameters);
                HttpResponseMessage response = await _httpClient.PostAsync("/api/v3/oauth/token", parametersContent);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    StravaToken tokens = JsonConvert.DeserializeObject<StravaToken>(result);
                    await _userManager.SetAuthenticationTokenAsync(user, STRAVA, ACCESS_TOKEN, tokens.access_token);
                    await _userManager.SetAuthenticationTokenAsync(user, STRAVA, REFRESH_TOKEN, tokens.refresh_token);
                    await _userManager.SetAuthenticationTokenAsync(user, STRAVA, EXPIRES_AT,
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tokens.expires_at).ToString("o"));
                    return tokens.access_token;
                }
            }
            return accessToken;
        }

        private string GetDateParameters(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            {
                TimeSpan start = dateTime - new DateTime(1970, 1, 1);
                int secondsStart = (int)start.TotalSeconds;
                dateTime = dateTime.AddDays(1);
                TimeSpan end = dateTime - new DateTime(1970, 1, 1);
                int secondsEnd = (int)end.TotalSeconds;
                return "after=" + secondsStart + "&before=" + secondsEnd + "&";
            }
            return "";
        }

        public async Task<StravaAuthResponse> IsUserStravaAuthAsync(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null)
            {
                return new StravaAuthResponse { ResponseType = ResponseType.Unauthorization };
            }

            var accessToken = await GetTokenAsync(user);
            return new StravaAuthResponse
            {
                ResponseType = ResponseType.OK,
                IsAuth = accessToken != null
            };
        }

        public async Task<ResponseType> DeleteStravaAuthAsync(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null)
            {
                return ResponseType.Unauthorization;
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, STRAVA, ACCESS_TOKEN);
            await _userManager.RemoveAuthenticationTokenAsync(user, STRAVA, REFRESH_TOKEN);
            await _userManager.RemoveAuthenticationTokenAsync(user, STRAVA, EXPIRES_AT);
            return ResponseType.OK;
        }

        private string GetDistanceString(double distance)
        {
            var km = Math.Round(distance / 1000, 2);
            return $"{km}km";
        }
    }
}
