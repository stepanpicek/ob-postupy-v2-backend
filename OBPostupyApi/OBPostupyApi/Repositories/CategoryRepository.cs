using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RepositoryContext _context;

        public CategoryRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(string raceKey)
        {
            return await _context.Categories
                .Where(c => c.Race.Key == raceKey)
                .Include(c => c.PersonResults)
                .ThenInclude(c => c.SplitTimes)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesWithCourseAsync(string raceKey)
        {
            return await _context.Categories
                .Where(c => c.Race.Key == raceKey)
                .Include(c => c.Course)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesWithSplitsAsync(string raceKey)
        {
            return await _context.Categories
                .Where(c => c.Race.Key == raceKey)
                .Include(c => c.PersonResults)
                    .ThenInclude(s => s.SplitTimes)
                .ToListAsync();
        }
    }
}
