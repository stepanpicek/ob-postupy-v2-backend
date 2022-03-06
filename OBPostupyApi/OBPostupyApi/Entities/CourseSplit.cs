namespace OBPostupyApi.Entities
{
    public class CourseSplit
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int SplitId { get; set; }
        public Split Split { get; set; }
        public int Order { get; set; }
    }
}
