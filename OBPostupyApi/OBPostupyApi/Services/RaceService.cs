using Microsoft.AspNetCore.Identity;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        private readonly UserManager<User> _userManager;
        public RaceService(IRaceRepository raceRepository, UserManager<User> userManager)
        {
            _raceRepository = raceRepository;
            _userManager = userManager;
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

            return new EditRaceResponse
            {
                ResponseType = ResponseType.OK,
                Type = race.Type,
                Key = raceKey,
                Date = race.StartTime,
                Name = race.Name,
                UserId = race.UserId
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
            if (user?.Id != race?.UserId && !await _userManager.IsInRoleAsync(user, Role.Admin.ToString()))
            {
                return false;
            }

            return true;
        }
    }
}
