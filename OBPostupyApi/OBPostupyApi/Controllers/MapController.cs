using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IRaceService _raceService;
        private readonly IMapService _mapService;
        private readonly ILogger<MapController> _logger;

        public MapController(IRaceService raceService, IMapService mapService, ILogger<MapController> logger)
        {
            _raceService = raceService;
            _mapService = mapService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadMapModel model)
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

            var response = await _mapService.SaveMapAsync(
                model.RaceKey,
                model.File.FileName, 
                model.File.OpenReadStream());

            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost("calibration")]
        public async Task<IActionResult> Calibration([FromBody] CalibrationMapModel model)
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

            MapData mapData = new MapData
            {
                East = model.East,
                West = model.West,
                South = model.South,
                North = model.North,
                Rotation = model.Rotation
            };

            var response = await _mapService.CalibrateMapAsync(model.RaceKey, mapData);
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

            var canUpload = await _raceService.CanUserEdit(raceKey, User);
            if (!canUpload)
            {
                return Unauthorized();
            }

            var response = await _mapService.DeleteMapAsync(raceKey);

            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("info/{key}")]
        public async Task<IActionResult> GetInfo(string key)
        {
            var response = await _mapService.GetMapInfoAsync(key);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("image/{key}")]
        public async Task<IActionResult> GetImage(string key)
        {
            var response = await _mapService.GetMapImageAsync(key);
            var bytes = response.ImageStream.ToArray();
            response.ImageStream.Dispose();

            return response.ResponseType switch
            {
                ResponseType.OK => File(bytes, "image/jpeg"),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }
    }
}
