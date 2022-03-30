using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OBPostupyApi.Models
{
    public class UploadResultModel
    {
        public string RaceKey { get; set; }
        public IFormFile File { get; set; }
    }
}
