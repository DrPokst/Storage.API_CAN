using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Storage.API.DTOs
{
    public class BomForCreationDto
    {
        public IFormFile File { get; set; }
        public DateTime DateAdded { get; set; }
        public string User { get; set; }

        public BomForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}