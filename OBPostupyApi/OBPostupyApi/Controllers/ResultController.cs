using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly IRaceService _raceService;

        [HttpPost("oris")]
        public async Task<IActionResult> UploadOris([FromBody] OrisResultModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var canUpload = await _raceService.CanUserEdit(model.RaceKey, User);
            if (!canUpload)
            {
                return Unauthorized();
            }

            var response = await _resultService.SaveOrisResultsAsync(model.RaceKey, model.OrisId);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadResultModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var canUpload = await _raceService.CanUserEdit(model.RaceKey, User);
            if (!canUpload)
            {
                return Unauthorized();
            }

            var response = await _resultService.SaveResultsAsync(model.RaceKey, model.File.OpenReadStream());
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
