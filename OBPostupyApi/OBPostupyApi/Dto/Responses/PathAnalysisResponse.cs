using System;
using OBPostupyApi.Enums;

namespace OBPostupyApi.Dto.Responses
{
    public class PathAnalysisResponse
    {
        public ResponseType ResponseType { get; set; }
        public double AverageSpeed { get; set; }
        public double AverageTempo { get; set; }
        public double Distance { get; set; }
        public double Ascension { get; set; }
        public double Descent { get; set; }
        public double Time { get; set; }
    }
}

