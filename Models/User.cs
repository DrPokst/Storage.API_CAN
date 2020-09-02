using System;


namespace Storage.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte [] PasswordHash { get; set; }
        public byte [] PasswordSalt { get; set; }
        public string Interests { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }
       
    }
}