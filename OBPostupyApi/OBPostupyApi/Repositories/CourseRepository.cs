using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly RepositoryContext _context;

        public CourseRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<CourseData> GetCourseDataByRaceAsync(string raceKey)
        {
            return await _context.CourseData
                .Where(c => c.Race.Key == raceKey)
                .Include(c => c.Courses)
                    .ThenInclude(c => c.CourseControl)
                        .ThenInclude(c => c.Control)
                .Include(c => c.Courses)
                    .ThenInclude(c => c.CourseSplits)
                        .ThenInclude(c => c.Split)
                .FirstOrDefaultAsync();
        }
    }
}
