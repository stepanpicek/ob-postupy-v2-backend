using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IAuthService
    {
        Task<ResponseType> UpdateProfilAsync(ClaimsPrincipal user, UpdateProfileModel model);
        Task<ProfileResponse> GetProfileAsync(ClaimsPrincipal user, string userId = null);
        Task<ResponseType> AddAminRoleAsync(ClaimsPrincipal user, string userId = null);
        Task<ResponseType> RemoveAminRoleAsync(ClaimsPrincipal user, string userId = null);
        UsersResponse GetAllUsers();
    }
}
