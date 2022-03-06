using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService; 
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IAuthService authService, ILogger<ProfileController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.UpdateProfilAsync(User, model);

            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _authService.GetProfileAsync(User);

            return result?.ResponseType switch
            {
                ResponseType.OK => Ok(result),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            var result = await _authService.GetProfileAsync(User, id);

            return result?.ResponseType switch
            {
                ResponseType.OK => Ok(result),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            var result = _authService.GetAllUsers();

            return result?.ResponseType switch
            {
                ResponseType.OK => Ok(result),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
