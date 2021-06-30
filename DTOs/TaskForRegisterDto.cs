using Microsoft.AspNetCore.Http;

namespace Storage.API.DTOs
{
    public class TaskForRegisterDto
    {
        public string Name { get; set; }
        public string BomName { get; set; }
        public int QTY { get; set; }
    }
}