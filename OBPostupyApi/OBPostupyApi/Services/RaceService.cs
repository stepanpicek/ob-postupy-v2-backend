using Microsoft.AspNetCore.Identity;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Repositories;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapService _mapService;
        private readonly IResultService _resultService;
        private readonly ICourseRepository _courseRepository;
        public RaceService(IRaceRepository raceRepository, UserManager<User> userManager, IMapService mapService, IResultService resultService, ICourseRepository courseRepository)
        {
            _raceRepository = raceRepository;
            _userManager = userManager;
            _mapService = mapService;
            _resultService = resultService;
            _courseRepository = courseRepository;
        }

        public async Task<EditRaceResponse> GetRaceToEditAsync(string raceKey, ClaimsPrincipal userClaims)
        {
            User user = await _userManager.GetUserAsync(userClaims);
            Race race = await _raceRepository.GetRaceByKeyAsync(raceKey);

            if (race == null || user == null)
            {
                return new EditRaceResponse
                {
                    ResponseType = ResponseType.BadRequest
                };
            }

            if (!await CanUserEdit(race, user))
            {
                return new EditRaceResponse
                {
                    ResponseType = ResponseType.Unauthorization
                };
            }

            var categories = await _resultService.GetCategoriesAsync(raceKey);
            var mapInfo = await _mapService.GetMapInfoAsync(raceKey);
            var courseData = await _courseRepository.GetCourseDataByRaceAsync(raceKey);

            return new EditRaceResponse
            {
                ResponseType = ResponseType.OK,
                Type = race.Type,
                Key = raceKey,
                Date = race.StartTime,
                Name = race.Name,
                UserId = race.UserId,
                Results = new EditRaceResultResponse
                {
                    IsUploaded = categories.ResponseType == ResponseType.OK &&
                        categories.Categories != null &&
                        categories.Categories.Count > 0
                },
                Map = new EditRaceMapResponse
                {
                    IsUploaded = mapInfo.ResponseType == ResponseType.OK,
                    IsCalibrated = mapInfo.Position != null &&
                        mapInfo.Position.Count > 0 &&
                        mapInfo.Position.All(p => p.Lat != 0 && p.Lon != 0)
                },
                CourseData = new EditRaceCourseDataResponse
                {
                    IsUploaded = courseData != null && courseData.Courses != null && courseData.Courses.Count > 0,
                    AreCoursesConnected = categories.ResponseType == ResponseType.OK &&
                        categories.Categories != null &&
                        categories.Categories.All(c => c.CourseId != null)
                }
            };
        }

        public async Task<string> CreateRaceAsync(CreateRaceModel model, User user)
        {
            Race race = new Race
            {
                Key = Guid.NewGuid().ToString("N"),
                Name = model.Name,
                StartTime = model.Date,
                Type = model.Type,
                User = user,
                UserId = user.Id
            };
            await _raceRepository.CreateRaceAsync(race);
            await _raceRepository.SaveAsync();
            return race.Key;
        }

        public async Task<bool> CanUserEdit(string raceKey, ClaimsPrincipal userClaims)
        {
            User user = await _userManager.GetUserAsync(userClaims) ?? new User();
            Race race = await _raceRepository.GetRaceByKeyAsync(raceKey) ?? new Race();
            return await CanUserEdit(race, user);
        }
        public async Task<bool> CanUserEdit(Race race, User user)
        {
            if (user == null || race == null)
            {
                return false;
            }

            if (user?.Id != race?.UserId && !await _userManager.IsInRoleAsync(user, Role.Admin.ToString()))
            {
                return false;
            }

            return true;
        }

        public async Task<RacesResponse> GetPublicRacesAsync()
        {
            var races = await _raceRepository.GetAllPublicRacesAsync();
            if(races == null)
            {
                return new RacesResponse { ResponseType = ResponseType.BadRequest };
            }

            var racesResponse = races.Select(r => new RaceResponse
            {
                Date = r.StartTime,
                Name = r.Name,
                Key = r.Key,
                Organizer = r.Organizer,
                OrisId = r.OrisId
            }).ToList();

            return new RacesResponse
            {
                ResponseType = ResponseType.OK,
                Races = racesResponse
            };
        }

        public async Task<ResponseType> GetRaceToShowAsync(string raceKey, ClaimsPrincipal userClaims)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if(race == null)
            {
                return ResponseType.BadRequest;
            }
            
            if(race.Type != RaceType.Private)
            {
                return ResponseType.OK;
            }

            if(race.Type == RaceType.Private)
            {
                var user = await _userManager.GetUserAsync(userClaims);

                if(await CanUserEdit(race, user))
                {
                    return ResponseType.OK;
                }
                return ResponseType.Unauthorization;
            }

            return ResponseType.BadRequest;
        }

        public async Task<RacesResponse> GetUserRacesAsync(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if(user == null)
            {
                return new RacesResponse { ResponseType = ResponseType.Unauthorization };
            }

            var races = await _raceRepository.GetAllUserRacesAsync(user.Id);
            if (races == null)
            {
                return new RacesResponse { ResponseType = ResponseType.BadRequest };
            }

            var racesResponse = races.Select(r => new RaceResponse
            {
                Date = r.StartTime,
                Name = r.Name,
                Key = r.Key,
                Organizer = r.Organizer,
                OrisId = r.OrisId,
                Type = r.Type.ToString()
            }).ToList();

            return new RacesResponse
            {
                ResponseType = ResponseType.OK,
                Races = racesResponse
            };
        }

        public async Task<RacesResponse> GetUserParticipatingRacesAsync(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null)
            {
                return new RacesResponse { ResponseType = ResponseType.Unauthorization };
            }

            if (string.IsNullOrEmpty(user.RegNumber))
            {
                return new RacesResponse { ResponseType = ResponseType.BadRequest };
            }

            var races = await _raceRepository.GetAllUserRacesByRegNumberAsync(user.RegNumber);
            if (races == null)
            {
                return new RacesResponse { ResponseType = ResponseType.BadRequest };
            }

            var racesResponse = races.Select(r => new RaceResponse
            {
                Date = r.StartTime,
                Name = r.Name,
                Key = r.Key,
                Organizer = r.Organizer,
                OrisId = r.OrisId
            }).ToList();

            return new RacesResponse
            {
                ResponseType = ResponseType.OK,
                Races = racesResponse
            };
        }

        public async Task<ResponseType> DeleteRaceAsync(string raceKey, ClaimsPrincipal userClaims)
        {
            var race = await _raceRepository.GetRaceByKeyAsync(raceKey);
            if (race == null)
            {
                return ResponseType.BadRequest;
            }

            if (!await CanUserEdit(raceKey, userClaims))
            {
                return ResponseType.Unauthorization;
            }

            await _mapService.DeleteMapAsync(raceKey);
            await _raceRepository.DeleteRaceAsync(raceKey);

            return ResponseType.OK;
        }

        public async Task<ResponseType> UpdateRaceAsync(UpdateRaceModel model, ClaimsPrincipal userClaims)
        {
            User user = await _userManager.GetUserAsync(userClaims);
            Race race = await _raceRepository.GetRaceByKeyAsync(model.RaceKey);

            if (race == null || user == null)
            {
                return ResponseType.BadRequest;
            }

            if (!await CanUserEdit(race, user))
            {
                return ResponseType.Unauthorization;
            }

            race.Name = model.Name;
            race.StartTime = model.Date;
            race.Type = model.Type;

            await _raceRepository.SaveAsync();
            return ResponseType.OK;
        }

        public async Task<AllRacesResponse> GetAllRacesAsync(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null || !await _userManager.IsInRoleAsync(user, Role.Admin.ToString()))
            {
                return new AllRacesResponse { ResponseType = ResponseType.Unauthorization };
            }

            var races = await _raceRepository.GetAllRacesAsync();
            if (races == null)
            {
                return new AllRacesResponse { ResponseType = ResponseType.BadRequest };
            }

            var racesResponse = races.Select(r => new ExtendedRaceResponse
            {
                Date = r.StartTime,
                Name = r.Name,
                Key = r.Key,
                Organizer = r.Organizer,
                Type = r.Type.ToString(),
                UserEmail = r.User?.Email,
                UserName = $"{r.User?.FirstName} {r.User?.LastName}",
            }).ToList();

            return new AllRacesResponse
            {
                ResponseType = ResponseType.OK,
                Races = racesResponse
            };
        }
    }
}
