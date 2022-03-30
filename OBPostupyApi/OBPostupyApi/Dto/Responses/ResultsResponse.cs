using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class ResultsResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<CategoryResultsResponse> Categories { get; set; }
    }
}
