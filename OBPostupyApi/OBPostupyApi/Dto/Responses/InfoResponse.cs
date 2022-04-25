using OBPostupyApi.Enums;

namespace OBPostupyApi.Dto.Responses
{
    public class InfoResponse
    {
        public ResponseType ResponseType { get; set; }
        public string UserManualFile { get; set; }
        public string OrganizerManualFile { get; set; }
        public string Info { get; set; }
    }
}
