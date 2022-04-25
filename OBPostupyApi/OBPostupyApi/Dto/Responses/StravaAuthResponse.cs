using OBPostupyApi.Enums;

namespace OBPostupyApi.Dto.Responses
{
    public class StravaAuthResponse
    {
        public ResponseType ResponseType { get; set; }
        public bool IsAuth { get; set; }
    }
}
