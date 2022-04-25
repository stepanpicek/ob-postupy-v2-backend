using OBPostupyApi.Entities;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Readers
{
    public class MapData
    {
        public double? West { get; set; }
        public double? East { get; set; }
        public double? North { get; set; }
        public double? South { get; set; }
        public double? Rotation { get; set; }
        public List<Position> Corners { get; set; }
        public string KmzImagePath { get; set; }
    }
}
