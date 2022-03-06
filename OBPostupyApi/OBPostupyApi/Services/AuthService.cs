using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<ResponseType> AddAminRoleAsync(ClaimsPrincipal userClaims, string userId)
        {
            User user = await GetUserByIdAuthorized(userClaims, userId);

            if (user == null)
            {
                return ResponseType.Unauthorization;
            }

            var result = await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
            if (!result.Succeeded)
            {
                return ResponseType.BadRequest;
            }

            return ResponseType.OK;
        }

        public async Task<ResponseType> RemoveAminRoleAsync(ClaimsPrincipal userClaims, string userId = null)
        {
            User user = await GetUserByIdAuthorized(userClaims, userId);

            if (user == null)
            {
                return ResponseType.Unauthorization;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, Role.Admin.ToString());
            if (!result.Succeeded)
            {
                return ResponseType.BadRequest;
            }

            return ResponseType.OK;
        }

        public async Task<ProfileResponse> GetProfileAsync(ClaimsPrincipal userClaims, string userId = null)
        {
            User user = userId == null ?
                await _userManager.GetUserAsync(userClaims) :
                await GetUserByIdAuthorized(userClaims, userId);

            var response = new ProfileResponse();

            if (user == null)
            {
                response.ResponseType = ResponseType.Unauthorization;
                return response;
            }

            response = new ProfileResponse
            {
                ResponseType = ResponseType.OK,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NickName = user.NickName,
                RegNumber = user.RegNumber,
                Birthdate = user.Birthdate == default(DateTime) ? null : user.Birthdate,
            };
            return response;
        }

        public async Task<ResponseType> UpdateProfilAsync(ClaimsPrincipal userClaims, UpdateProfileModel model)
        {
            User user = model.UserId == null ? 
                await _userManager.GetUserAsync(userClaims) : 
                await GetUserByIdAuthorized(userClaims, model.UserId);

            if (user == null)
            {
                return ResponseType.Unauthorization;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.RegNumber = model.RegNumber;
            user.NickName = model.NickName;
            if (model.Birthdate.HasValue)
            {
                user.Birthdate = model.Birthdate.Value;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return ResponseType.OK;
            }

            return ResponseType.BadRequest;
        }

        public UsersResponse GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            
            var usersResponse = users.Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                IsAdmin = _userManager.IsInRoleAsync(u, Role.Admin.ToString()).GetAwaiter().GetResult(),
            }).ToList();

            return new UsersResponse
            {
                ResponseType = ResponseType.OK,
                Users = usersResponse
            };
        }

        private async  Task<User> GetUserByIdAuthorized(ClaimsPrincipal userClaims, string userId)
        {
            var actualUser = await _userManager.GetUserAsync(userClaims);
            var roles = await _userManager.GetRolesAsync(actualUser);
            if (actualUser == null || (actualUser.Email != "mail@stepanpicek.cz" && !roles.Contains(Role.Admin.ToString())))
            {
                return null;
            }
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
