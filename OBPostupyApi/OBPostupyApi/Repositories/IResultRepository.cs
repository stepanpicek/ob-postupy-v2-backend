using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface IResultRepository
    {
        Task<PersonResult> GetPersonResultAsync(int id);
        Task<List<Category>> GetCategoriesAsync(string raceKey);
        Task<Category> GetCategoryResultByIdAsync(int id);
        Task DeleteResultsAsync(string raceKey);
    }
}
