using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MapController(IRaceService raceService, IMapService mapService, IWebHostEnvironment hostingEnvironment)
        {
            _raceService = raceService;
            _mapService = mapService;
            _hostingEnvironment = hostingEnvironment;
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
                _hostingEnvironment.WebRootPath, 
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
    }
}
