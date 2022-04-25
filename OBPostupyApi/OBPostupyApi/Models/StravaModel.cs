using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OBPostupyApi.Models
{
    public class SummaryActivity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("moving_time")]
        public long MovingTime { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
    }

    public class StreamSet
    {
        [JsonProperty("latlng")]
        public Latlng Latlng { get; set; }

        [JsonProperty("distance")]
        public Distance Distance { get; set; }

        [JsonProperty("time")]
        public Distance Time { get; set; }
    }

    public class Distance
    {
        [JsonProperty("data")]
        public List<double> Data { get; set; }

        [JsonProperty("series_type")]
        public string SeriesType { get; set; }

        [JsonProperty("original_size")]
        public long OriginalSize { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }
    }

    public class Latlng
    {
        [JsonProperty("data")]
        public List<List<double>> Data { get; set; }

        [JsonProperty("series_type")]
        public string SeriesType { get; set; }

        [JsonProperty("original_size")]
        public long OriginalSize { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }
    }

    public class StravaToken
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public double expires_at { get; set; }
        public double expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
