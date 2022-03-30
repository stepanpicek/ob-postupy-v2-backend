using OBPostupyApi.Dto.Readers;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface IMapService
    {
        Task<ResponseType> SaveMapAsync(string raceKey, string fileName, Stream fileStream);
        Task<ResponseType> CalibrateMapAsync(string raceKey, MapData mapData);
        Task<MapInfoResponse> GetMapInfoAsync(string raceKey);
        Task<MapImageResponse> GetMapImageAsync(string raceKey);
        Task<ResponseType> DeleteMapAsync(string raceKey);
    }
}
