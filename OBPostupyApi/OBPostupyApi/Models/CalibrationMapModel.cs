using OBPostupyApi.Dto.Responses;
using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class CalibrationMapModel
    {
        public string RaceKey { get; set; }
        public double? Rotation { get; set; }
        public List<PositionResponse> Corners { get; set; }
        public double East { get; set; }
        public double West { get; set; }
        public double North { get; set; }
        public double South { get; set; }
    }
}
