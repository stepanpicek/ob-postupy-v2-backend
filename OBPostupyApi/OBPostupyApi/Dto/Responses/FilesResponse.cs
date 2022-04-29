using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class FilesResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<FileResponse> Files { get; set; }
    }

    public class FileResponse
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }
}
