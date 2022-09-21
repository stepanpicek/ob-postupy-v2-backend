using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using OBPostupyApi.Services;
using System.Threading.Tasks;

namespace OBPostupyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PathController : ControllerBase
    {
        private readonly IPathService _pathService;

        public PathController(IPathService pathService)
        {
            _pathService = pathService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] UploadPathModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _pathService.SavePathAsync(model.PersonResultId, model.Path);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _pathService.RemovePathAsync(id);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost("draw")]
        public async Task<IActionResult> Draw([FromBody] DrawPathModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _pathService.DrawPathAsync(model.PersonResultId, model.SplitPaths);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("by-result/{id}")]
        public async Task<IActionResult> GetPathByPersonResult(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _pathService.GetPathAsync(id.Value);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("with-speeds/{id}")]
        public async Task<IActionResult> GetPathWithSpeeds(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _pathService.GetPathWithSpeedAsync(id.Value);
            return response.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpGet("analysis/{id}")]
        public async Task<IActionResult> GetPathAnalysis(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _pathService.GetPathAnalysisAsync(id.Value);
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
