using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Storage.API.Models;

namespace Storage.API.DTOs
{
    public class ComponetsForRegisterDto
    {   
        public string Mnf { get; set; }
        public string Manufacturer { get; set; }
        public string Detdescription { get; set; }
        public string BuhNr { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Nominal { get; set; }
        public string Furl { get; set; }
        public string Durl { get; set; }
        public string Murl { get; set; }
        public DateTime Created { get; set; }
        public string PublicId { get; set; }
        public IFormFile file { get; set; }

    }
}