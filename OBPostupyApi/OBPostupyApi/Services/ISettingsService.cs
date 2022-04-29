using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using System.IO;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface ISettingsService
    {
        Task<ResponseType> SaveFile(Stream file, string fileName);
        Task<ResponseType> DeleteFile(int id);
        Task<FilesResponse> GetFiles();
        Task<ResponseType> UpdateInfo(string info);
        Task<InfoResponse> GetInfo();
    }
}
