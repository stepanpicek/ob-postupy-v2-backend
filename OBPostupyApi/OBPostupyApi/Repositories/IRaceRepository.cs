using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface IRaceRepository
    {
        public Task CreateRaceAsync(Race race);
        public Task<Race> GetRaceByKeyAsync(string key);
        public Task<List<Race>> GetAllPublicRacesAsync();
        public Task<List<Race>> GetAllOrisRacesInMonthAsync();
        public Task<List<Race>> GetAllUserRacesAsync(string id);
        public Task<List<Category>> GetCategoriesAsync(string key);
        public Task DeleteRaceAsync(string key);
        public Task SaveAsync();
    }
}
