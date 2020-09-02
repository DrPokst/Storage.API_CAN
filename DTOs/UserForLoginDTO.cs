using System.ComponentModel.DataAnnotations;

namespace Storage.API.DTOs
{

    public class UserForLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}