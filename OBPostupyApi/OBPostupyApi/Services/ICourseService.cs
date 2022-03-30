using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface ICourseService
    {
        Task<ResponseType> SaveCoursesAsync(string raceKey, Stream fileStream);
        Task<ResponseType> AddCoursesToCategoriesAsync(string raceKey, List<CourseToCategory> courseToCategories);
        Task<CourseResponse> GetCourseAsync(int id);
    }
}
