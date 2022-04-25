using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Extensions;
using OBPostupyApi.Readers;
using OBPostupyApi.Repositories;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class MapService : IMapService
    {
        private readonly IAnalysisService _analysisService;
        private readonly IRaceRepository _raceRepository;
        private readonly IMapRepository _mapRepository;
        private readonly IMapReader _mapReader;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<MapService> _logger;

        private readonly HashSet<string> _imgExtensions = new HashSet<string>{ ".jpg", ".png" };
        private readonly HashSet<string> _gisExtensions = new HashSet<string> { ".kmz" };

        public MapService(IRaceRepository raceRepository, IMapRepository mapRepository, IMapReader mapReader, IAnalysisService analysisService,
            ILogger<MapService> logger, IWebHostEnvironment hostingEnvironment)
        {
            _raceRepository = raceRepository;
            _mapRepository = mapRepository;
            _mapReader = mapReader;
            _analysisService = analysisService;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ResponseType> CalibrateMapAsync(string raceKey, MapData mapData)
        {
            var map = await _mapRepository.GetMapByRaceAsync(raceKey);
            if (map == null)
            {
                return ResponseType.BadRequest;
            }

            map.SetCorners(mapData);
            await _raceRepository.SaveAsync();
            return ResponseType.OK;
        }

        public async Task<ResponseType> SaveMapAsync(string raceKey, string fileName, Stream fileStream)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            var mapExtension = System.IO.Path.GetExtension(fileName)?.ToLowerInvariant();
            Map map = await _mapRepository.GetMapByRaceAsync(raceKey) ?? new Map();
            if (_gisExtensions.Contains(mapExtension)) ProcessKmz(fileStream, _hostingEnvironment.WebRootPath, map);
            if (_imgExtensions.Contains(mapExtension)) await ProcessImg(fileName, fileStream, _hostingEnvironment.WebRootPath, map);
            
            fileStream.Dispose();
            if (map != null)
            {
                race.Maps = new List<Map> { map };
                await _raceRepository.SaveAsync();
                return ResponseType.OK;
            }

            return ResponseType.BadRequest;
        }

        private Map ProcessKmz(Stream fileStream, string rootPath, Map map)
        {
            var mapData = _mapReader.ReadKmz(fileStream);
            var path = _mapReader.SaveKmzImage(fileStream, mapData?.KmzImagePath, GetUploadDirectory(rootPath));
            map.PathToFile = path;
            map.SetCorners(mapData);
            return map; 
        }

        private async Task<Map> ProcessImg(string fileName, Stream fileStream, string rootPath, Map map)
        {
            var path = await _mapReader.SaveImageAsync(fileStream, fileName, GetUploadDirectory(rootPath));
            map.PathToFile = path;
            return map;
        }

        private string GetUploadDirectory(string root)
        {
            var completePath = System.IO.Path.Combine("uploads", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
            var uploadPath = System.IO.Path.Combine(root, completePath);
            DirectoryInfo di = Directory.CreateDirectory(uploadPath);
            return completePath;
        }

        public async Task<MapInfoResponse> GetMapInfoAsync(string raceKey)
        {
            var map = await _mapRepository.GetMapByRaceAsync(raceKey);
            if (map == null)
            {
                return new MapInfoResponse { ResponseType = ResponseType.BadRequest };
            }
            return new MapInfoResponse
            {
                ResponseType = ResponseType.OK,
                Position = map.Corners.Select(c => new PositionResponse(c.lat, c.lon)).ToList(),
                Scale = map.Scale,
            };
        }

        public async Task<MapImageResponse> GetMapImageAsync(string raceKey)
        {
            var map = await _mapRepository.GetMapByRaceAsync(raceKey);
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, map?.PathToFile ?? "");
            
            if (map == null || !File.Exists(path))
            {
                return new MapImageResponse { ResponseType = ResponseType.BadRequest };
            }

            var image = _analysisService.GetMapWithWaterMark(path);
            return new MapImageResponse
            {
                ResponseType = ResponseType.OK,
                Image = image
            };
        }

        public async Task<ResponseType> DeleteMapAsync(string raceKey)
        {
            var map = await _mapRepository.GetMapByRaceAsync(raceKey);
            if(map == null)
            {
                return ResponseType.BadRequest;
            }

            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, map?.PathToFile ?? "");
            if (File.Exists(path))
            {
                Policy.Handle<Exception>()
                   .WaitAndRetry(10, retryAttempt => TimeSpan.FromMilliseconds(500), (exception, time) =>
                   {
                       _logger.LogWarning(exception, "Error during deleting map file.");
                   })
                   .Execute(() => File.Delete(path));
            }

            await _mapRepository.DeleteMapAsync(map);

            return ResponseType.OK;
        }
    }
}
