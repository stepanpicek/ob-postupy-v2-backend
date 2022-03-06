using System;

namespace OBPostupyApi.Entities
{
    public class SplitTime
    {
        public int Id { get; set; }
        public int? SplitId { get; set; }
        public Split Split { get; set; }
        public DateTime Time { get; set; }
        public int TimeSpan { get; set; }
        public string Code { get; set; }
        public int PersonResultId { get; set; }
        public PersonResult PersonResult { get; set; }
    }
}
