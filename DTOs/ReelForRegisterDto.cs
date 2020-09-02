using Microsoft.AspNetCore.Http;

namespace Storage.API.DTOs
{
    public class ReelForRegisterDto
    {
        public string CMnf { get; set; }
        public int QTY { get; set; }
        public string PublicId { get; set; }
        public IFormFile file { get; set; }
    }
}