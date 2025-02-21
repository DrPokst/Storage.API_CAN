using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Storage.API.Models
{
    public class Reel
    {
        [Key]
        public int Id { get; set; }
        public string CMnf { get; set; }
        public int QTY { get; set; }
        public ICollection<Photo2> Photos2 { get; set; }
        public string? Location { get; set; }
        public ICollection<History> History { get; set; }
        public int ComponentasId { get; set; }
        public int? UserId { get; set; }
    }
}