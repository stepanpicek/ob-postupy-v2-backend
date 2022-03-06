using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class CourseController : ControllerBase
    {
        private readonly IRaceService _raceService;
        private readonly ICourseService _courseService;

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadCourseModel model)
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


            var response = await _courseService.SaveCoursesAsync(model.RaceKey, model.File.OpenReadStream());
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
