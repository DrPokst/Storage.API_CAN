using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.DTOs
{
    public class PhotoForRegisterDto
    {
        public IFormFile file { get; set; }
    }
}
