using OBPostupyApi.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OBPostupyApi.Dto.Responses
{
    public class CategoryResultsResponse
    {
        public ResponseType ResponseType { get; set; }
        public int Id { get; set; }
        public string Category { get; set; }
        public List<PersonResultsResponse> People { get; set; }
    }

    public class PersonResultsResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Position { get; set; }
        public string Time { get => TimeValue.ToString(); }
        public string StartTime { get; set; }

        [JsonIgnore]
        public TimeSpan TimeValue { get; set; }
        public string TimeLoss { get; set; }
        public string RegNumber { get; set; }
        public string Status { get; set; }
        public bool IsPathUploaded { get; set; }
    }
}
