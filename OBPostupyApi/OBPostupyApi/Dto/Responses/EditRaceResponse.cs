using OBPostupyApi.Entities;
using OBPostupyApi.Enums;
using System;

namespace OBPostupyApi.Dto.Responses
{
    public class EditRaceResponse
    {
        public ResponseType ResponseType { get; set; }
        public string Key { get; set; }
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
