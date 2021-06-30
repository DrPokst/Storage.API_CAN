namespace Storage.API_CAN.DTOs
{
    public class BomListWithXQtyForListDto
    {
        public int Id { get; set; } 
        public string BuhNr { get; set; }
        public int xQty { get; set; }
        public int QtyInDb { get; set; }
        public int BomNameId { get; set; }

    }
}