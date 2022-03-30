using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class CourseToCategoryModel
    {
        public string RaceKey { get; set; }
        public List<CourseToCategory> CourseCategories { get; set; }
    }
}
