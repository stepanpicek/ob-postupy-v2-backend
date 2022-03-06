using OBPostupyApi.Enums;
using System.Collections.Generic;

namespace OBPostupyApi.Dto.Responses
{
    public class UsersResponse
    {
        public ResponseType ResponseType { get; set; }
        public List<UserResponse> Users { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
