using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface ISettingsService
    {
        Task<ResponseType> SaveOrganizerManual(Stream file);
        Task<ResponseType> SaveUserManual(Stream file); 
        Task<ResponseType> UpdateInfo(string info);
        Task<InfoResponse> GetInfo();
    }
}
