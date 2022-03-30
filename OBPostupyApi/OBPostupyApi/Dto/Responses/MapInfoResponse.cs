using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class MapInfoResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<PositionResponse> Position { get; set; }
        public double Scale { get; set; }
    }
}
