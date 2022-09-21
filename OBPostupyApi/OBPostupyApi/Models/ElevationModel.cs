using System;
using Newtonsoft.Json;
using System.Resources;

namespace OBPostupyApi.Models
{
    public class ElevationModel
    {
        [JsonProperty("authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty("brandLogoUri")]
        public Uri BrandLogoUri { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("resourceSets")]
        public ResourceSet[] ResourceSets { get; set; }

        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }
    }

    public class ResourceSet
    {
        [JsonProperty("estimatedTotal")]
        public long EstimatedTotal { get; set; }

        [JsonProperty("resources")]
        public Resource[] Resources { get; set; }

    }

    public class Resource
    {
        [JsonProperty("__type")]
        public string Type { get; set; }

        [JsonProperty("elevations")]
        public long[] Elevations { get; set; }

        [JsonProperty("zoomLevel")]
        public long ZoomLevel { get; set; }
    }
}