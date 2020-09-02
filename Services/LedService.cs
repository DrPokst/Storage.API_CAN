using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Device.Spi;
using Iot.Device.Mcp25xxx;
using Iot.Device.Mcp25xxx.Register;
using Iot.Device.Ws28xx;
using Iot.Device.Graphics;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Mcp25xxx.Register.CanControl;
using Iot.Device.Mcp25xxx.Register.MessageTransmit;

namespace Storage.API.Services
{
    public class LedService : ILedService
    {
       
        public async Task<bool> TurnOnLed(int id)
        {
            Run2(id);

            // var device = new Ws2812b(spi, 75);
    

            // // Display basic colors for 5 sec
            // BitmapImage img = device.Image;
            // img.Clear();
            // img.SetPixel(id-1, 0, Color.Blue);
            // device.Update();
            return true;
            
        }

        public async Task<bool> TurnOff(int id)
        {
            return true;
        }
        
        private static void Run2(int id)
        {
            Console.WriteLine("Hello Mcp25xxx Sample!");

            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                Reset(mcp25xxx);

                byte[] data = new byte[] { (byte)id, (byte)id, (byte)id, 4, 5, 6, 7, 8 };

                mcp25xxx.Write(Address.Cnf1, new byte [] { 0b0000_0000 });
                mcp25xxx.Write(Address.Cnf2, new byte [] { 0b1001_0001 });
                mcp25xxx.Write(Address.Cnf3, new byte [] { 0b0000_0001 });
                
                //ReadAllRegisters(mcp25xxx);
                //ReadAllRegistersWithDetails(mcp25xxx);
               // ReadRxBuffer(mcp25xxx);
                //Write(mcp25xxx);
                //LoadTxBuffer(mcp25xxx);
               // RequestToSend(mcp25xxx);
               // ReadStatus(mcp25xxx);
               // RxStatus(mcp25xxx);
               // BitModify(mcp25xxx);
                TransmitMessage(mcp25xxx, data);
               // LoopbackMode(mcp25xxx);
               // Write(mcp25xxx);
               // ReadAllRegisters(mcp25xxx);

                //mcp25xxx.Write(Address.CanCtrl, new byte[] { 0b1001_1111 });

            }
        }
        private static Mcp25xxx GetMcp25xxxDevice()
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
        private static void Reset(Mcp25xxx mcp25xxx)
        {
            Console.WriteLine("Reset Instruction");
            mcp25xxx.Reset();
        }

        private static void TransmitMessage(Mcp25xxx mcp25xxx,  byte[] data)
        {
            Console.WriteLine("Transmit Message");

            mcp25xxx.WriteByte(
                new CanCtrl(CanCtrl.PinPrescaler.ClockDivideBy8,
                    false,
                    false,
                    false,
                    Iot.Device.Mcp25xxx.Tests.Register.CanControl.OperationMode.NormalOperation));

            mcp25xxx.Write(
                Address.TxB0Sidh,
                new byte[]
                {
                    new TxBxSidh(0, 0b0000_0000).ToByte(), new TxBxSidl(0, 0b000, false, 0b001).ToByte(),
                    new TxBxEid8(0, 0b0000_0000).ToByte(), new TxBxEid0(0, 0b0000_0000).ToByte(),
                    new TxBxDlc(0, data.Length, false).ToByte()
                });

            mcp25xxx.Write(Address.TxB0D0, data);

            // Send with TxB0 buffer.
            mcp25xxx.RequestToSend(true, false, false);
        }
    }
}
