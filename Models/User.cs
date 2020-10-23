using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Storage.API_CAN.Models;

namespace Storage.API.Models
{
    public class User : IdentityUser<int>
    {
        public string Interests { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }
        public ICollection<History> History { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserPhoto> UserPhoto { get; set; }
       
    }
}