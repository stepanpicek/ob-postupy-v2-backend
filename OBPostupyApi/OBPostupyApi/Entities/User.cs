using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace OBPostupyApi.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RegNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string NickName { get; set; }

        private List<Race> CreatedRaces { get; set; }
    }
}
