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
    public class CourseController : ControllerBase
    {
        private readonly IRaceService _raceService;
        private readonly ICourseService _courseService;

        public CourseController(IRaceService raceService, ICourseService courseService)
        {
            _raceService = raceService;
            _courseService = courseService;
        }

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

        [HttpGet("course-category/{raceKey}")]
        public async Task<IActionResult> GetCourseToCategory(string raceKey)
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

            var response = await _courseService.GetCoursesToCategoriesAsync(raceKey);
            return response?.ResponseType switch
            {
                ResponseType.OK => Ok(response),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [HttpPost("course-category")]
        public async Task<IActionResult> CourseToCategory([FromBody] CourseToCategoryModel model)
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

            var response = await _courseService.AddCoursesToCategoriesAsync(model.RaceKey, model.CourseCategories);
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

            var response = await _courseService.DeleteCoursesAsync(raceKey);
            return response switch
            {
                ResponseType.OK => Ok(),
                ResponseType.BadRequest => BadRequest(),
                ResponseType.Unauthorization => Unauthorized(),
                _ => BadRequest()
            };
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var response = await _courseService.GetCourseAsync(id);
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
