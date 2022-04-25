using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OBPostupyApi.Dto.Responses;
using OBPostupyApi.Enums;
using OBPostupyApi.Repositories;
using Polly;
using System;
using System.IO;
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

        public async Task<ResponseType> SaveOrganizerManual(Stream file) => await SaveManual(file, ORGANIZER_MANUAL_PATH);

        public async Task<ResponseType> SaveUserManual(Stream file) => await SaveManual(file, USER_MANUAL_PATH);

        private async Task<ResponseType> SaveManual(Stream file, string fileName)
        {
            if (file == null)
            {
                return ResponseType.BadRequest;
            }

            var path = Path.Combine(_hostingEnvironment.WebRootPath, fileName);
            if (File.Exists(path))
            {
                Policy.Handle<Exception>()
                   .WaitAndRetry(10, retryAttempt => TimeSpan.FromMilliseconds(500), (exception, time) =>
                   {
                       _logger.LogWarning(exception, "Error during deleting manual file.");
                   })
                   .Execute(() => File.Delete(path));
            }

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            return ResponseType.OK;
        }

        public async Task<ResponseType> UpdateInfo(string info)
        {
            await _repository.UpdateInfo(info);
            return ResponseType.OK;
        }
    }
}
