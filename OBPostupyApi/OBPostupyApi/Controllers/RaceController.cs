using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class RaceController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRaceService _raceService;

        public RaceController(UserManager<User> userManager, IRaceService raceService)
        {
            _userManager = userManager;
            _raceService = raceService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRaceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userManager.GetUserAsync(User);
            var raceKey = await _raceService.CreateRaceAsync(model, user);

            return Ok(raceKey);
        }

        [HttpGet("edit/{key}")]
        public async Task<IActionResult> GetToEdit(string key)
        {
            var response = await _raceService.GetRaceToEditAsync(key, User);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("show/{key}")]
        public async Task<IActionResult> GetToShow(string key)
        {
            var response = await _raceService.GetRaceToShowAsync(key, User);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("all-public")]
        public async Task<IActionResult> GetAllPublicRaces()
        {
            var response = await _raceService.GetPublicRacesAsync();
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
