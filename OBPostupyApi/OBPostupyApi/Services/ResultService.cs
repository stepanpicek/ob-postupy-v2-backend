using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Readers;
using OBPostupyApi.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class ResultService : IResultService
    {
        private readonly IResultsReader _resultsReader;
        private readonly IRaceRepository _raceRepository;
        private readonly IResultRepository _resultRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResultService> _logger;

        public ResultService(IResultsReader resultsReader, HttpClient httpClient, IRaceRepository raceRepository, IResultRepository resultRepository, ILogger<ResultService> logger)
        {
            _resultsReader = resultsReader;
            _httpClient = httpClient;
            _raceRepository = raceRepository;
            _resultRepository = resultRepository;
            _logger = logger;
        }

        public async Task<ResponseType> DeleteResults(string raceId)
        {
            try
            {
                await _resultRepository.DeleteResultsAsync(raceId);
                return ResponseType.OK;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during deleting results");
                return ResponseType.BadRequest;
            }
        }

        public async Task<CategoriesResponse> GetCategoriesAsync(string raceId)
        {
            var categories = await _resultRepository.GetCategoriesAsync(raceId);
            if(categories == null)
            {
                return new CategoriesResponse { ResponseType = ResponseType.BadRequest };
            }

            var categoriesResponse = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                CourseId = c.CourseId,
                Name = c.Name
            }).OrderBy(c => c.Name).ToList();

            return new CategoriesResponse { ResponseType = ResponseType.OK, Categories = categoriesResponse };
        }

        public async Task<CategoryResultsResponse> GetCategoryResultsAsync(int id)
        {
            var results = await _resultRepository.GetCategoryResultByIdAsync(id);
            if (results == null)
            {
                return new CategoryResultsResponse { ResponseType = ResponseType.BadRequest };
            }

            var resultsResponse = results.PersonResults
                .OrderBy(p => p.Position)
                .Select(p => new PersonResultsResponse
                {
                    Id = p.Id,
                    FirstName = p.Person?.FirstName,
                    LastName = p.Person?.LastName,
                    Position = p.Position,
                    IsPathUploaded = p.Path != null,
                    Status = p.Status?.ToLowerInvariant(),
                    StartTime = p.StartTime.ToString("O")
                }).ToList();

            return new CategoryResultsResponse { ResponseType= ResponseType.OK, People = resultsResponse };
        }

        public async Task<ResultsResponse> GetRaceResults(string raceId)
        {
            var categories = await _raceRepository.GetCategoriesAsync(raceId);
            if(categories == null)
            {
                return new ResultsResponse { ResponseType = ResponseType.BadRequest };
            }

            var categoriesResponse = categories.OrderBy(c => c.Name).Select(c =>
            new CategoryResultsResponse
            {
                Category = c.Name,
                Id = c.Id,
                People = c.PersonResults.OrderBy(p => p.Position).Select(p => new PersonResultsResponse
                {
                    Id=p.Id,
                    FirstName = p.Person?.FirstName,
                    LastName = p.Person?.LastName,
                    Position = p.Position,
                    TimeValue = (p.FinishTime - p.StartTime),
                    RegNumber = p.Person.RegNumbers.FirstOrDefault(),
                    Status = p.Status.ToLowerInvariant()
                }).ToList()
            }).ToList();

            AddTimeLoss(categoriesResponse);
            return new ResultsResponse
            {
                ResponseType = ResponseType.OK,
                Categories = categoriesResponse
            };
        }

        private void AddTimeLoss(List<CategoryResultsResponse> categories)
        {
            foreach(var category in categories)
            {
                var first = category.People.FirstOrDefault();
                for(int i = 1; i < category.People.Count; i++)
                {
                    var loss = category?.People[i]?.TimeValue - first?.TimeValue;
                    if (loss.HasValue)
                    {
                        category.People[i].TimeLoss = $"+ {loss}";
                    }
                }
            }
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

        public async Task<SearchResultsResponse> SearchRaceResultsAsync(string raceId, string term)
        {
            var results = await _resultRepository.GetCategoriesWithResultsAsync(raceId);
            if(results == null || results.Count == 0)
            {
                return new SearchResultsResponse { ResponseType = ResponseType.BadRequest };
            }

            var categories = new List<SearchCategoryResultsResponse>();

            foreach (var result in results)
            {
                SearchCategoryResultsResponse category = null;
                foreach (var personResult in result.PersonResults)
                {
                    List<bool> expressions = new List<bool>
                    {
                        personResult?.Person?.RegNumbers?.Any(r => r?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ?? false,
                        personResult?.Person?.FirstName?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0,
                        personResult?.Person?.LastName?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0,
                    };

                    if (expressions.Any(e => e))
                    {
                        category ??= new SearchCategoryResultsResponse();
                        category.People ??= new List<SearchPersonResultsResponse>();
                        category.People.Add(new SearchPersonResultsResponse
                        {
                            FirstName = personResult?.Person?.FirstName,
                            LastName = personResult?.Person?.LastName,
                            RegNumber = personResult?.Person?.RegNumbers?.FirstOrDefault(),
                            Id = personResult.Id,
                            Status = personResult?.Status,
                            Position = personResult?.Position,
                        });
                    }
                }

                if (category != null || result.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    category ??= new SearchCategoryResultsResponse();
                    category.Id = result.Id;
                    category.Category = result.Name;
                    categories.Add(category);
                }
            }
            
            return new SearchResultsResponse 
            { 
                ResponseType = ResponseType.OK,
                Categories = categories 
            };
        }
    }
}
