using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class UploadPathModel
    {
        public int PersonResultId { get; set; }
        public List<PathData> Path { get; set; }
    }
}
