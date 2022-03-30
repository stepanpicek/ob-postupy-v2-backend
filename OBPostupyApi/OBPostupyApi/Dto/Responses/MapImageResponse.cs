using OBPostupyApi.Enums;
using System.IO;

namespace OBPostupyApi.Dto.Responses
{
    public class MapImageResponse
    {
        public ResponseType ResponseType { get; set; }
        public MemoryStream ImageStream { get; set; }
    }
}
