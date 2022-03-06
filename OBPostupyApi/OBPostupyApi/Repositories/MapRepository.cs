using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class MapRepository : IMapRepository
    {
        private readonly RepositoryContext _context;

        public MapRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Map> GetMapByRaceAsync(string raceKey)
        {
            return await _context.Maps.Where(m => m.Race.Key == raceKey).FirstOrDefaultAsync();
        }
    }
}
