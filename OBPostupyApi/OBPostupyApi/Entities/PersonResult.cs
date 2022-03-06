using System;
using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class PersonResult
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; }
        public List<SplitTime> SplitTimes { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int? PathId { get; set; }
        public Path Path { get; set; }
    }
}
