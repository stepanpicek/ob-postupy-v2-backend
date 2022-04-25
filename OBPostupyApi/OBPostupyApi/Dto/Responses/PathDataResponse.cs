using OBPostupyApi.Enums;
using OBPostupyApi.Models;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class PathDataResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<PathData> Locations { get; set; }
    }
}
