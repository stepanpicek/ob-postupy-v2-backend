using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface ICourseRepository
    {
        Task<CourseData> GetCourseDataByRaceAsync(string raceKey);
    }
}
a