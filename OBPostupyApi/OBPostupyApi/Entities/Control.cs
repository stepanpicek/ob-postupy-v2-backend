using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBPostupyApi.Entities
{
    public class Control
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public ControlType Type { get; set; }
        public string CoordinatesString { get; set; }
        public string MapCoordinatesString { get; set; }
        public int? CourseDataId { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseControl> CourseControl { get; set; }
        public List<Split> SplitsFirstControl { get; }
        public List<Split> SplitsSecondControl { get; }

        [NotMapped]
        public Tuple<double, double> Coordinates
        {
            get
            {
                if (CoordinatesString != null)
                {
                    string[] tab = CoordinatesString.Split(';');
                    return Tuple.Create(double.Parse(tab[0]), double.Parse(tab[1]));
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    CoordinatesString = string.Format("{0};{1}", value.Item1, value.Item2);
                }
            }
        }

        [NotMapped]
        public Tuple<double, double> MapCoordinates
        {
            get
            {
                if (MapCoordinatesString != null)
                {
                    string[] tab = MapCoordinatesString.Split(';');
                    return Tuple.Create(double.Parse(tab[0]), double.Parse(tab[1]));
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    MapCoordinatesString = string.Format("{0};{1}", value.Item1, value.Item2);
                }
            }
        }
    }
}
