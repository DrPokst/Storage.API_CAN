namespace Storage.API.Models
{
    public class Rxmsg
    {
        public int ID { get; set; }
        public int DLC { get; set; }    
        public byte[] Msg { get; set; }
    }
}