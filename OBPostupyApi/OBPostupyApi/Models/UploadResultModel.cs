using Microsoft.AspNetCore.Http;

namespace OBPostupyApi.Models
{
    public class UploadResultModel
    {
        public string RaceKey { get; set; }
        public IFormFile File { get; set; }
    }
}
