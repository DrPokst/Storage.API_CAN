namespace Storage.API_CAN.Models
{
    public class BomList
    {
        public int Id { get; set; } 
        public string BuhNr { get; set; }
        public int Qty { get; set; }
        public int BomNameId { get; set; }
        public int ComponentasId { get; set; }
    }
}