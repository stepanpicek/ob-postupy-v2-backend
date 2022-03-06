using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBPostupyApi.Entities
{
    public class Race
    {
        public int Id { get; set; }
        public string Key { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Category> Categories { get; set; }
        public List<Map> Maps { get; set; }
        public CourseData CourseData { get; set; }
        public string Organizer { get; set; }
        public int OrisId { get; set; }
    }
}
