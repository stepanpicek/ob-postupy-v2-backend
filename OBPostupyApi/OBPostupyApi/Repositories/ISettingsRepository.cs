using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface ISettingsRepository
    {
        Task UpdateInfo(string info);
        Task<Setting> GetInfo();
        Task SaveFile(string path);
        Task<List<Setting>> GetFiles();
        Task<Setting> GetFile(int id);
        Task DeleteFile(int id);
        Task DeleteFile(Setting setting);
    }
}
