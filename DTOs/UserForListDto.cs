using System;

namespace Storage.API_CAN.DTOs
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime LastActive { get; set; }
    }
}