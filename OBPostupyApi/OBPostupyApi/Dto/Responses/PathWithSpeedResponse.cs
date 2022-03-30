using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class PathWithSpeedResponse : PathResponse
    {
        public List<double> Speed { get; set; }
    }
}
