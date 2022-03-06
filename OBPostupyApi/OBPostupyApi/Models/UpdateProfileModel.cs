using System;

namespace OBPostupyApi.Models
{
    public class UpdateProfileModel : RegisterModel
    {
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string RegNumber { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
