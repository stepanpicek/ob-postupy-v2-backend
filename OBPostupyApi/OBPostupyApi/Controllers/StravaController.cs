using AspNet.Security.OAuth.Strava;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OBPostupyApi.Enums;
using OBPostupyApi.Repositories;
using OBPostupyApi.Services;
using OBPostupyApi.Settings;
using System.Threading.Tasks;
using System;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class StravaController : ControllerBase
    {
        private readonly IStravaService _stravaService;
        private readonly IUserRepository _userRepository;
        private readonly FrontEndSettings _frontEndSettings;

        public StravaController(IStravaService stravaService, IUserRepository userRepository, IOptions<FrontEndSettings> options)
        {
            _stravaService = stravaService;
            _userRepository = userRepository;
            _frontEndSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [AllowAnonymous]
        [HttpGet("auth/{id}")]
        public async Task<IActionResult> StravaAuth(string id)
        {
            if (id == null)
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetUserByIdAsync(id);
            if(user == null)
            {
                return Unauthorized();
            }

            return new ChallengeResult(
                StravaAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(StravaCallback), new { userId = user.Id })
                });
        }

        [AllowAnonymous]
        [HttpGet(nameof(StravaCallback))]
        public async Task<IActionResult> StravaCallback(string userId)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(StravaAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return BadRequest();
            }

            var tokens = authenticateResult.Properties.GetTokens();
            var response = await _stravaService.SetAuthTokensAsync(userId, tokens);
            return response switch
            {
                ResponseType.OK => Redirect($"{_frontEndSettings.Uri}/ucet/profil"),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("activities/{date}")]
        public async Task<IActionResult> GetActivities(string date)
        {
            var response = await _stravaService.GetActivityListAsync(date, User);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("activity/{id}")]
        public async Task<IActionResult> GetActivity(long id)
        {
            var response = await _stravaService.GetActivityAsync(id, User);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet]
        public async Task<IActionResult> IsUserAuth()
        {
            var response = await _stravaService.IsUserStravaAuthAsync(User);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStravaAuth()
        {
            var response = await _stravaService.DeleteStravaAuthAsync(User);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
