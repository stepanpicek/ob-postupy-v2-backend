using OBPostupyApi.Enums;
using System;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class RacesResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<RaceResponse> Races { get; set; }
    }

    public class RaceResponse
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Organizer { get; set; }
        public int OrisId { get; set; }
    }
}
