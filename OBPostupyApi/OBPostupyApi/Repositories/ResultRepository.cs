using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class ResultRepository : IResultRepository
    {
        private readonly RepositoryContext _context;

        public ResultRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(string raceKey)
        {
            return await _context.Categories
                .Where(c => c.Race.Key == raceKey)
                .ToListAsync();
        }

        public async Task<PersonResult> GetPersonResultAsync(int id)
        {
            return await _context.PersonResults
                .Where(pr => pr.Id == id)
                .Include(pr => pr.SplitTimes)
                .ThenInclude(st => st.Split)
                .ThenInclude(sp => sp.SecondControl)
                .Include(pr => pr.SplitTimes)
                .ThenInclude(st => st.Split)
                .ThenInclude(sp => sp.FirstControl)
                .SingleOrDefaultAsync();
        }

        public async Task<Category> GetCategoryResultByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Include(c => c.PersonResults)
                    .ThenInclude(pr => pr.Person)
                .Include(c => c.PersonResults)
                    .ThenInclude(pr => pr.Path)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task DeleteResultsAsync(string raceKey)
        {
            Race race = await _context.Races.Where(r => r.Key == raceKey)
                .Include(r => r.Categories)
                    .ThenInclude(c => c.PersonResults)
                    .ThenInclude(p => p.SplitTimes)
                .Include(r => r.Categories)
                    .ThenInclude(c => c.PersonResults)
                    .ThenInclude(p => p.Person)
                .FirstOrDefaultAsync();

            if (race == null) return;

            foreach (var c in race.Categories)
            {
                foreach (var p in c.PersonResults)
                {
                    _context.Person.RemoveRange(p.Person);
                }

                foreach (var p in c.PersonResults)
                {
                    _context.SplitTimes.RemoveRange(p.SplitTimes);
                }
                _context.PersonResults.RemoveRange(c.PersonResults);
            }
            _context.Categories.RemoveRange(race.Categories);

            await _context.SaveChangesAsync();
        }
    }
}
