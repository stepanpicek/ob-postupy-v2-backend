using OBPostupyApi.Enums;
using System;

namespace OBPostupyApi.Dto.Responses
{
    public class ProfileResponse
    {
        public ResponseType ResponseType { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string RegNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public bool IsStravaConnected { get; set; }
    }
}
