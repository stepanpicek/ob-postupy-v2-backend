using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class PathRepository : IPathRepository
    {
        private readonly RepositoryContext _context;

        public PathRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Path> GetPathByIdAsync(int pathId)
        {
            return await _context.Paths
                .Where(p => p.Id == pathId)
                .Include(p => p.Locations)
                .SingleOrDefaultAsync();
        }

        public async Task<Path> GetPathByResultIdAsync(int resultId)
        {
            return await _context.Paths
                .Where(p => p.PersonResult.Id == resultId)
                .Include(p => p.Locations)
                .Include(p => p.PersonResult)
                .SingleOrDefaultAsync();
        }

        public async Task RemovePathAsync(Path path)
        {
            if (path != null)
            {
                PersonResult personResult = path.PersonResult;
                personResult.PathId = null;
                personResult.Path = null;

                _context.Paths.Remove(path);
                _context.PersonResults.Update(personResult);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
