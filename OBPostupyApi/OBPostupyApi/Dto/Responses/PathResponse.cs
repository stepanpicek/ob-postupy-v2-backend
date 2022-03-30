using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class PathResponse
    {
        public ResponseType ResponseType { get; set; }
        public int PersonResultId { get; set; }
        public List<List<double>> Locations { get; set; }
    }
}
