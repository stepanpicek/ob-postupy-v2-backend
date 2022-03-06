using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IMapService
    {
        Task<ResponseType> SaveMapAsync(string raceKey, string rootPath, string fileName, Stream fileStream);
        Task<ResponseType> CalibrateMapAsync(string raceKey, MapData mapData);
    }
}
