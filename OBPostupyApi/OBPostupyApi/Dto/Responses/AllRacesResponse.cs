using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class AllRacesResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<ExtendedRaceResponse> Races { get; set; }
    }

    public class ExtendedRaceResponse : RaceResponse
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
