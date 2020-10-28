using System;
using System.Collections.Generic;

namespace Storage.API_CAN.Models
{
    public class BomName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<BomList> BomList { get; set; }
    }
}