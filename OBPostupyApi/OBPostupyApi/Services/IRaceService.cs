using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IRaceService
    {
        Task<string> CreateRaceAsync(CreateRaceModel model, User user);
        Task<EditRaceResponse> GetRaceToEditAsync(string raceKey, ClaimsPrincipal userClaims);
        Task<bool> CanUserEdit(string raceKey, ClaimsPrincipal userClaims);
        Task<bool> CanUserEdit(Race race, User user);
    }
}
