using System;
using Microsoft.AspNetCore.Http;

namespace Storage.API.DTOs
{
    public class TaskForUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string BomName { get; set; }
        public int Qty { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }
    }
}