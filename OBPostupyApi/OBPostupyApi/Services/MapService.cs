using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Extensions;
using OBPostupyApi.Readers;
using OBPostupyApi.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class MapService : IMapService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IMapRepository _mapRepository;
        private readonly IMapReader _mapReader;

        private readonly HashSet<string> _imgExtensions = new HashSet<string>{ ".jpg", ".png" };
        private readonly HashSet<string> _gisExtensions = new HashSet<string> { ".kmz" };

        public MapService(IRaceRepository raceRepository, IMapRepository mapRepository, IMapReader mapReader)
        {
            _raceRepository = raceRepository;
            _mapRepository = mapRepository;
            _mapReader = mapReader;
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

        public async Task<ResponseType> SaveMapAsync(string raceKey, string rootPath, string fileName, Stream fileStream)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            var mapExtension = System.IO.Path.GetExtension(fileName)?.ToLowerInvariant();
            Map map = await _mapRepository.GetMapByRaceAsync(raceKey) ?? new Map();
            if (_gisExtensions.Contains(mapExtension)) ProcessKmz(fileStream, rootPath, map);
            if (_imgExtensions.Contains(mapExtension)) await ProcessImg(fileName, fileStream, rootPath, map);
            
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
            var path = _mapReader.SaveKmzImage(fileStream, mapData?.KmzImagePath, rootPath);
            map.PathToFile = path;
            map.SetCorners(mapData);
            return map; 
        }

        private async Task<Map> ProcessImg(string fileName, Stream fileStream, string rootPath, Map map)
        {
            var path = await _mapReader.SaveImageAsync(fileStream, fileName, rootPath);
            map.PathToFile = path;
            return map;
        }
    }
}
