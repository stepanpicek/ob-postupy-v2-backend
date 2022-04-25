using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IRaceService
    {
        Task<string> CreateRaceAsync(CreateRaceModel model, User user);
        Task<ResponseType> UpdateRaceAsync(UpdateRaceModel model, ClaimsPrincipal userClaims);
        Task<ResponseType> DeleteRaceAsync(string raceKey, ClaimsPrincipal userClaims);
        Task<EditRaceResponse> GetRaceToEditAsync(string raceKey, ClaimsPrincipal userClaims);
        Task<ResponseType> GetRaceToShowAsync(string raceKey, ClaimsPrincipal userClaims);
        Task<RacesResponse> GetPublicRacesAsync();
        Task<RacesResponse> GetUserRacesAsync(ClaimsPrincipal userClaims);
        Task<RacesResponse> GetUserParticipatingRacesAsync(ClaimsPrincipal userClaims);
        Task<AllRacesResponse> GetAllRacesAsync(ClaimsPrincipal userClaims);
        Task<bool> CanUserEdit(string raceKey, ClaimsPrincipal userClaims);
        Task<bool> CanUserEdit(Race race, User user);
    }
}
