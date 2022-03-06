using OBPostupyApi.Entities;
using System;

namespace OBPostupyApi.Models
{
    public class CreateRaceModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public RaceType Type { get; set; }
    }
}
