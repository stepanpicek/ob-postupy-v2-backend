using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class PathService : IPathService
    {
        private readonly IAnalysisService _analysisService;
        private readonly IPathRepository _pathRepository;
        private readonly IResultRepository _resultRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<PathService> _logger;

        public PathService(IAnalysisService analysisService, IPathRepository pathRepository, IResultRepository resultRepository, ICourseRepository courseRepository, ILogger<PathService> logger)
        {
            _analysisService = analysisService;
            _pathRepository = pathRepository;
            _resultRepository = resultRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<ResponseType> DrawPathAsync(int personResultId, List<SplitPath> pathData)
        {
            var personResult = await _resultRepository.GetPersonResultAsync(personResultId);
            if (personResult == null)
            {
                return ResponseType.BadRequest;
            }

            var splits = pathData.OrderBy(p => p.Order)
                .Select(p => p.Positions.Select(ps => new Location { Position = Tuple.Create(ps.Lat, ps.Lon) }).ToList())
                .ToList();

            personResult.Path = new Path
            {
                Locations = _analysisService.GetDrawnPath(splits, personResult)
            };

            await _pathRepository.SaveAsync();

            return ResponseType.OK;
        }

        public async Task<PathWithSpeedResponse> GetPathWithSpeedAsync(int personResultId)
        {
            var personResult = await _resultRepository.GetPersonResultAsync(personResultId);
            var response = new PathWithSpeedResponse
            {
                PersonResultId = personResultId
            };

            if (personResult == null)
            {
                response.ResponseType = ResponseType.BadRequest;
                return response;
            }
            var interpolated = await GetInterpolatedLocationsAsync(personResult);
            var speed = _analysisService.GetSpeed(interpolated);
            response.ResponseType = ResponseType.OK;
            response.Locations = interpolated.Select(l => new List<double> { l.Position.Item1, l.Position.Item2 }).ToList();
            response.Speed = speed;
            return response;
        }

        public async Task<PathResponse> GetPathAsync(int personResultId)
        {
            var personResult = await _resultRepository.GetPersonResultAsync(personResultId);
            var response = new PathResponse
            {
                PersonResultId = personResultId
            };

            if (personResult == null)
            {
                response.ResponseType = ResponseType.BadRequest;
                return response;
            }
            var interpolated = await GetInterpolatedLocationsAsync(personResult);

            response.ResponseType = ResponseType.OK;
            response.Locations = interpolated.Select(l => new List<double> { l.Position.Item1, l.Position.Item2 }).ToList();
            return response;
        }

        private async Task<List<Location>> GetInterpolatedLocationsAsync(PersonResult personResult)
        {
            var locations = (await _pathRepository.GetPathByResultIdAsync(personResult.Id))?.Locations ??
               GetEmptyPath(personResult, await _courseRepository.GetCourseByCategoryIdAsync(personResult.CategoryId));
            return _analysisService.InterpolationByTime(locations, 1);
        }

        private List<Location> GetEmptyPath(PersonResult personResult, Course course)
        {
            List<Location> pathLocations = new List<Location>();
            var splitTimes = personResult.SplitTimes.OrderBy(st => st.Time).ToList();
            var controls = course.CourseControl.OrderBy(cc => cc.Order).ToList();

            pathLocations.Add(new Location { Position = controls.First().Control.Coordinates, Time = personResult.StartTime });

            for (int i = 0; i < splitTimes.Count; i++)
            {
                if (i + 1 < controls.Count)
                {
                    pathLocations.Add(new Location { Position = controls[i + 1].Control.Coordinates, Time = splitTimes[i].Time });
                }
            }
            pathLocations.Add(new Location { Position = controls.Last().Control.Coordinates, Time = personResult.FinishTime });
            return pathLocations;
        }

        public async Task<ResponseType> RemovePathAsync(int personResultId)
        {
            var path = await _pathRepository.GetPathByResultIdAsync(personResultId);
            if (path == null)
            {
                return ResponseType.BadRequest;
            }
            await _pathRepository.RemovePathAsync(path);
            return ResponseType.OK;
        }

        public async Task<ResponseType> SavePathAsync(int personResultId, List<PathData> pathData)
        {
            var personResult = await _resultRepository.GetPersonResultAsync(personResultId);
            if(personResult == null)
            {
                return ResponseType.BadRequest;
            }

            var locations = pathData.Select(p => new Location
            {
                Time = p.Timestamp,
                Position = Tuple.Create(p.Lat, p.Lon)
            }).ToList();

            var interpolatedData = _analysisService.InterpolationByTime(locations, 6);
            personResult.Path = new Path
            {
                Locations = locations
            };
            await _pathRepository.SaveAsync();

            return ResponseType.OK;
        }

        public async Task<ResponseType> DeletePathAsync(int personResultId)
        {
            var path = await _pathRepository.GetPathByResultIdAsync(personResultId);
            if(path == null)
            {
                return ResponseType.BadRequest;
            }

            await _pathRepository.RemovePathAsync(path);
            return ResponseType.OK;
        }
    }
}
