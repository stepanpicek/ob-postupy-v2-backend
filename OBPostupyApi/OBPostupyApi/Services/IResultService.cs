using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IResultService
    {
        Task<ResponseType> SaveOrisResultsAsync(string raceId, string orisId);
        Task<ResponseType> SaveResultsAsync(string raceId, Stream fileStream);
    }
}
