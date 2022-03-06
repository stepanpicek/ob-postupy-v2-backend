using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CourseDataId { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseControl> CourseControl { get; set; }
        public List<CourseSplit> CourseSplits { get; set; }
        public List<Category> Categories { get; }
    }
}
