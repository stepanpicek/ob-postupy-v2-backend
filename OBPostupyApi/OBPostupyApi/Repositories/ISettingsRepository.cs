using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface ISettingsRepository
    {
        Task UpdateInfo(string info);
        Task<Setting> GetInfo();
    }
}
