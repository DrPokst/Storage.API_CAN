using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Mnf { get; set; }
        public int OldQty { get; set; }
        public int NewQty { get; set; }
        public string OldLocation { get; set; }
        public string NewLocation { get; set; }
        public DateTime DateAdded { get; set; }
        public int ComponentasId { get; set; }
        public int ReelId { get; set; }

    }
}
