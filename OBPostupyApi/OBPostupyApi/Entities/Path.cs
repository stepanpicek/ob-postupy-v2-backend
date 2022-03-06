using Newtonsoft.Json;
using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class Path
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Location> Locations { get; set; }

        [JsonIgnore]
        public PersonResult PersonResult { get; set; }
    }
}
