using OBPostupyApi.Enums;
using OBPostupyApi.Readers;
using OBPostupyApi.Repositories;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class ResultService : IResultService
    {
        private readonly IResultsReader _resultsReader;
        private readonly IRaceRepository _raceRepository;
        private readonly HttpClient _httpClient;

        public ResultService(IResultsReader resultsReader, HttpClient httpClient, IRaceRepository raceRepository)
        {
            _resultsReader = resultsReader;
            _httpClient = httpClient;
            _raceRepository = raceRepository;
        }

        public async Task<ResponseType> SaveOrisResultsAsync(string raceId, string orisId)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceId);
            if(race == null)
            {
                return ResponseType.BadRequest;
            }

            _httpClient.BaseAddress = new Uri("https://oris.orientacnisporty.cz/");
            var response = await _httpClient.GetAsync("ExportVysledkuXML?id=" + orisId + "&v=3");
            if (response.IsSuccessStatusCode)
            {
                var results = _resultsReader.Read(await response.Content.ReadAsStreamAsync());
                if(results != null) {
                    race.Categories = results;
                    await _raceRepository.SaveAsync();
                    return ResponseType.OK;
                }
            }

            return ResponseType.BadRequest;
        }

        public async Task<ResponseType> SaveResultsAsync(string raceId, Stream fileStream)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceId);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            var results = _resultsReader.Read(fileStream);
            if (results != null)
            {
                race.Categories = results;
                await _raceRepository.SaveAsync();
                return ResponseType.OK;
            }

            return ResponseType.BadRequest;
        }
    }
}
