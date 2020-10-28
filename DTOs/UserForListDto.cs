using System;
using System.Collections.Generic;
using Storage.API.Models;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.DTOs
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<UserPhoto> UserPhoto { get; set; }
        public ICollection<History> History { get; set; }
        public ICollection<Reel> Reels { get; set; }

    }
}