using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBPostupyApi.Entities
{
    public class Location
    {
        public long Id { get; set; }

        [JsonIgnore]
        public string PositionString { get; set; }
        public DateTime? Time { get; set; }
        public double? Elevation { get; set; }

        [JsonIgnore]
        public int PathId { get; set; }

        [JsonIgnore]
        public Path Path { get; set; }

        [NotMapped]
        public Tuple<double, double> Position
        {
            get
            {
                string[] tab = PositionString.Split(';');
                return Tuple.Create(double.Parse(tab[0]), double.Parse(tab[1]));
            }
            set
            {
                PositionString = string.Format("{0};{1}", value.Item1, value.Item2);
            }
        }
    }
}
