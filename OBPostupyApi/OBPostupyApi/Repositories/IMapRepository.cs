using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface IMapRepository
    {
        Task<Map> GetMapByRaceAsync(string raceKey);
        Task DeleteMapAsync(Map map);
    }
}
