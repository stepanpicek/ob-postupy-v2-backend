using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class Split
    {
        public int Id { get; set; }
        public int? FirstControlId { get; set; }
        public Control FirstControl { get; set; }
        public int? SecondControlId { get; set; }
        public Control SecondControl { get; set; }
        public int? CourseDataId { get; set; }
        public CourseData CourseData { get; set; }
        public List<CourseSplit> CourseSplits { get; set; }
        public List<SplitTime> SplitTimes { get; }
    }
}
