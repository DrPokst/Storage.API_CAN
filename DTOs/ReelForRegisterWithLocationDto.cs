using Microsoft.AspNetCore.Http;

namespace Storage.API.DTOs
{
    public class ReelForRegisterWithLocationDto
    {
        public string CMnf { get; set; }
        public int QTY { get; set; }
        public string PublicId { get; set; }
        public string Location { get; set; }
        public IFormFile file { get; set; }
        public string URL { get; set; }
    }
}