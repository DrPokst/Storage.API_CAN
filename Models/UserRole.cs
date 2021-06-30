using Microsoft.AspNetCore.Identity;
using Storage.API.Models;

namespace Storage.API_CAN.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public AppRole Role { get; set; }
        
    }
}