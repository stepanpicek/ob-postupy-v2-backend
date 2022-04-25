using OBPostupyApi.Enums;
using System;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class StravaActivitiesResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<StravaActivityResponse> Activities { get; set; }
    }

    public class StravaActivityResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Distance { get; set; }
        public TimeSpan Time { get; set; }
    }
}
