using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;
        private readonly ILogger<RoleController> _logger;
        public RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger, IAuthService authService)
        {
            _roleManager = roleManager;
            _logger = logger;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {
            var roles = Enum.GetValues(typeof(Role)).Cast<Role>();
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }

            return Ok();
        }

        [HttpPost("add-admin")]
        [Authorize]
        public async Task<IActionResult> AddAdminRole([FromBody] AdminRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.AddAminRoleAsync(User, model?.UserId);

            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost("remove-admin")]
        [Authorize]
        public async Task<IActionResult> RemoveAdminRole([FromBody] AdminRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RemoveAminRoleAsync(User, model?.UserId);

            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
