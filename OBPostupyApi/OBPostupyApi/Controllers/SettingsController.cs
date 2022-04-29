using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _settingsService.SaveFile(file?.OpenReadStream(), file?.FileName);
            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpDelete("file/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _settingsService.DeleteFile(id);
            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _settingsService.GetFiles();
            return result.ResponseType switch
            {
                ResponseType.OK => Ok(result),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost("update-info")]
        public async Task<IActionResult> UpdateInfo([FromForm] UpdateInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _settingsService.UpdateInfo(model.Info);
            return result switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("info")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _settingsService.GetInfo();

            return result.ResponseType switch
            {
                ResponseType.OK => Ok(result),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
