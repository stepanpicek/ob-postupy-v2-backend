using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Repositories;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISettingsRepository _repository;
        private readonly ILogger<SettingsService> _logger;
        private const string ORGANIZER_MANUAL_PATH = "organizer-manual.pdf";
        private const string USER_MANUAL_PATH = "user-manual.pdf";

        public SettingsService(IWebHostEnvironment hostingEnvironment, ISettingsRepository repository, ILogger<SettingsService> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _repository = repository;
            _logger = logger;
        }

        public async Task<InfoResponse> GetInfo()
        {
            var userManualPath = Path.Combine(_hostingEnvironment.WebRootPath, USER_MANUAL_PATH);
            var organizerManualPath = Path.Combine(_hostingEnvironment.WebRootPath, ORGANIZER_MANUAL_PATH);
            return new InfoResponse
            {
                ResponseType = ResponseType.OK,
                Info = (await _repository.GetInfo())?.Value,
                UserManualFile = File.Exists(userManualPath) ? userManualPath : null,
                OrganizerManualFile = File.Exists(organizerManualPath) ? organizerManualPath : null,
            };
        }

        public async Task<ResponseType> SaveFile(Stream file, string fileName)
        {
            if (file == null)
            {
                return ResponseType.BadRequest;
            }
            var fileNm = fileName;
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "files", fileNm);
            while (File.Exists(path))
            {
                fileNm = $"{Path.GetRandomFileName()}{fileName}";
                path = Path.Combine(_hostingEnvironment.WebRootPath, "files", fileNm);
            }

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            await _repository.SaveFile(fileNm);

            return ResponseType.OK;
        }

        public async Task<ResponseType> UpdateInfo(string info)
        {
            await _repository.UpdateInfo(info);
            return ResponseType.OK;
        }

        public async Task<ResponseType> DeleteFile(int id)
        {
            var file = await _repository.GetFile(id);
            
            if(file == null || file.Value == null)
            {
                return ResponseType.BadRequest;
            }

            var path = Path.Combine(_hostingEnvironment.WebRootPath, "files", file.Value);
            if (File.Exists(path))
            {
                Policy.Handle<Exception>()
                   .WaitAndRetry(10, retryAttempt => TimeSpan.FromMilliseconds(500), (exception, time) =>
                   {
                       _logger.LogWarning(exception, "Error during deleting map file.");
                   })
                   .Execute(() => File.Delete(path));
            }
            await _repository.DeleteFile(file);
            return ResponseType.OK;
        }

        public async Task<FilesResponse> GetFiles()
        {
            var files = await _repository.GetFiles();
            return new FilesResponse
            {
                ResponseType = ResponseType.OK,
                Files = files.Select(f => new FileResponse { Id = f.Id, Path = f.Value}).ToList()
            };
        }
    }
}
