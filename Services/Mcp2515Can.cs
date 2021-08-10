using System.Device.Spi;
using Iot.Device.Mcp25xxx;

namespace Storage.API_CAN.Services
{
    public class Mcp2515Can
    {
        public Mcp25xxx mcp25Xxx { get; set; }
        public static Mcp25xxx InitSPI()
        {
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 10_000_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };
            var spiDevice = SpiDevice.Create(settings);
            return new Mcp25625(spiDevice);
        }


        
    }
}
