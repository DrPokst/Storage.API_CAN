using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Storage.API_CAN.Models
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}