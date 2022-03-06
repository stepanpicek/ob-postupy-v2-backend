using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public int? RaceId { get; set; }
        public Race Race { get; set; }
        public List<PersonResult> PersonResults { get; set; }
    }
}
