using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBPostupyApi.Entities
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string PathToFile { get; set; }
        public int Scale { get; set; }
        public int? RaceId { get; set; }
        public Race Race { get; set; }
        public string CornersString { get; set; }
        public double? Rotation { get; set; }
        public double East { get; set; }
        public double West { get; set; }
        public double North { get; set; }
        public double South { get; set; }

        [NotMapped]
        public List<Position> Corners
        {
            get
            {
                string[] data = CornersString.Split(';');
                var coorners = new List<Position>();
                foreach (var coordinates in data)
                {
                    string[] coordinate = coordinates.Split('|');
                    double lat;
                    double lon;
                    if (coordinate.Length == 2 && double.TryParse(coordinate[0], out lat) && double.TryParse(coordinate[1], out lon))
                    {
                        coorners.Add(new Position(lat, lon));
                    }
                }
                return coorners;
            }
            set
            {
                string data = "";
                foreach (var coord in value)
                {
                    data += coord.lat + "|" + coord.lon + ";";
                }
                CornersString = data;
            }
        }
    }
}
