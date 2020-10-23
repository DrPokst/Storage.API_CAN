using System;
using System.Collections.Generic;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.DTOs
{
    public class UserForListAdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public string Email { get; set; }   
        public DateTime LastActive { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public ICollection<UserPhoto> UserPhoto { get; set; }
    }
} 