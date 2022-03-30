using OBPostupyApi.Dto.Responses;
using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class SplitPath
    {
        public int Order { get; set; }
        public List<PositionResponse> Positions { get; set; }
    }
}
