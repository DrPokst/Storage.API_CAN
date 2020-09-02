using System.ComponentModel.DataAnnotations;

namespace Storage.API.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Slaptazodis turi buti nuo 4 iki 8 simboliu")]
        public string Password { get; set; }
    }
}