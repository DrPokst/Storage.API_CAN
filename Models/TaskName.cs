using System;
using System.Collections.Generic;

namespace Storage.API_CAN.Models
{
    public class TaskName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public BomName BomName { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<TaskList> TaskList { get; set; }
    }
}