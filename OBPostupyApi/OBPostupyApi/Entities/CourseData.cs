using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class CourseData
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }
        public List<Control> Controls { get; set; }
        public List<Split> Splits { get; set; }
        public List<Course> Courses { get; set; }
    }
}
