using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<ResultController> _logger;

        public ResultController(IResultService resultService, IRaceService raceService, ILogger<ResultController> logger)
        {
            _resultService = resultService;
            _raceService = raceService;
            _logger = logger;
        }

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

        [HttpDelete("{raceKey}")]
        public async Task<IActionResult> Delete(string raceKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var canEdit = await _raceService.CanUserEdit(raceKey, User);
            if (!canEdit)
            {
                return Unauthorized();
            }

            var response = await _resultService.DeleteResults(raceKey);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("{raceKey}")]
        public async Task<IActionResult> GetResults(string raceKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var canEdit = await _raceService.CanUserEdit(raceKey, User);
            if (!canEdit)
            {
                return Unauthorized();
            }

            var response = await _resultService.GetRaceResults(raceKey);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("categories/{key}")]
        public async Task<IActionResult> GetCategories(string key)
        {
            var response = await _resultService.GetCategoriesAsync(key);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetGategoryResults(int id)
        {
            var response = await _resultService.GetCategoryResultsAsync(id);
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
