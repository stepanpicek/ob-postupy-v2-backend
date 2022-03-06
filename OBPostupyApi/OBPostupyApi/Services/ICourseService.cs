using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface ICourseService
    {
        Task<ResponseType> SaveCoursesAsync(string raceKey, Stream fileStream);
    }
}
