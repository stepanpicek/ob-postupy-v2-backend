using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync(string raceKey);
        Task<List<Category>> GetCategoriesWithCourseAsync(string raceKey);
    }
}
