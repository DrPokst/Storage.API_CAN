namespace Storage.API_CAN.DTOs
{
    public class TaskListForReportDto
    {
        public int Id { get; set; } 
        public string BuhNr { get; set; }
        public string ManufPartNr { get; set; }
        public int Qty { get; set; }
        public int QtyInDb { get; set; }
        public int BomNameId { get; set; }
        public int ComponentasId { get; set; }
        public string Status { get; set; }
        public int TaskNameId { get; set; }
    }
}