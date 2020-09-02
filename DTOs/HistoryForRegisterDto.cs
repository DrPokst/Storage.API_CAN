using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.DTOs
{
    public class HistoryForRegisterDto
    {
        public int Id { get; set; }
        public string Mnf { get; set; }
        public int NewQty { get; set; }
        public string NewLocation { get; set; }
    }
}
