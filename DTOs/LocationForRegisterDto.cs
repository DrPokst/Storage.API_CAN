namespace Storage.API.DTOs
{
    public class LocationForRegisterDto
    {
        public int Id { get; set; }
        public string Mnf { get; set; }
        public string Location { get; set; }
        public int QTY { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}