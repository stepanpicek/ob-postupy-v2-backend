namespace OBPostupyApi.Entities
{
    public class CourseControl
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int ControlId { get; set; }
        public Control Control { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }
}
