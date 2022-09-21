using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OBPostupyApi.Entities;
using OBPostupyApi.Models;
using OBPostupyApi.Settings;

namespace OBPostupyApi.Services
{
    public class ElevationService : IElevationService
    {
        private readonly VirtualEarthSettings _settings;
        private readonly ILogger<ElevationService> _logger;
        private readonly HttpClient _httpClient;

        public ElevationService(HttpClient httpClient, IOptions<VirtualEarthSettings> settings, ILogger<ElevationService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SetElevationAsync(List<Location> locations)
        {
            string coordinates = GetCoordinatesString(locations);
            ElevationModel elevationModel = await GetElevationAsync(coordinates);
            SetElevation(elevationModel, locations);
        }

        private async Task<ElevationModel> GetElevationAsync(string coordinates)
        {
            var points = "points=" + coordinates;
            var data = new StringContent(points);
            var response = await _httpClient.PostAsync($"Elevation/List?key={_settings.ApiKey}", data);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ElevationModel>(responseString);
            return responseObject;
        }

        private string GetCoordinatesString(List<Location> locations)
        {
            StringBuilder locationBuilder = new StringBuilder();
            var last = locations.Last();
            foreach (var location in locations)
            {
                if (location != null)
                {
                    locationBuilder.Append(location.Position.Item1.ToString(CultureInfo.InvariantCulture));
                    locationBuilder.Append(',');
                    locationBuilder.Append(location.Position.Item2.ToString(CultureInfo.InvariantCulture));
                }
                if (!location.Equals(last))
                {
                    locationBuilder.Append(',');
                }
            }
            return locationBuilder.ToString();
        }

        private void SetElevation(ElevationModel model, List<Location> locations)
        {
            if (model != null && model?.ResourceSets != null && model.ResourceSets?.FirstOrDefault()?.Resources != null)
            {
                var elevations = model.ResourceSets.First().Resources.First().Elevations;

                for (int i = 0; i < locations.Count; i++)
                {
                    if (i >= elevations.Length)
                    {
                        locations[i].Elevation = elevations.Last();
                        continue;
                    }
                    locations[i].Elevation = elevations[i];
                }
            }
        }
    }
}

