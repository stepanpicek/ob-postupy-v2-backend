using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class CourseResponse
    {
        public ResponseType ResponseType { get; set; }
        public int? Category { get; set; }
        public List<ControlResponse> Controls { get; set; }
    }

    public class ControlResponse
    {
        public int Id { get; set; }
        public List<double> Position { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
    }
}
