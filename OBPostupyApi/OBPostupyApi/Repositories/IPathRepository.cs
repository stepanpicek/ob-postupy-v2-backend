using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface IPathRepository
    {
        Task<Path> GetPathByIdAsync(int pathId);
        Task<Path> GetPathByResultIdAsync(int resultId);
        Task RemovePathAsync(Path path);
        Task SaveAsync();
    }
}
