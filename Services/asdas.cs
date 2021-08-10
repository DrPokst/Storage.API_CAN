using System;
using System.Device.Spi;
using Iot.Device.Mcp25xxx;
using Iot.Device.Mcp25xxx.Register;
using Iot.Device.Mcp25xxx.Register.CanControl;
using Iot.Device.Mcp25xxx.Register.MessageTransmit;

namespace Storage.API_CAN.Services
{
    public static class McpCan
    {
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
        private static void InitMcp(Mcp25xxx mcp25xxx)
        {
            mcp25xxx.Write(Address.Cnf1, new byte[] { 0b0000_0000 });
            mcp25xxx.Write(Address.Cnf2, new byte[] { 0b1001_0001 });
            mcp25xxx.Write(Address.Cnf3, new byte[] { 0b0000_0001 });
        }
        private static void TransmitMessage(Mcp25xxx mcp25xxx, byte[] data)
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
                    new TxBxSidh(0, 0b0000_0000).ToByte(), new TxBxSidl(0, 0b000, false, 0b000).ToByte(),
                    new TxBxEid8(0, 0b0000_0000).ToByte(), new TxBxEid0(0, 0b0000_0000).ToByte(),
                    new TxBxDlc(0, data.Length, false).ToByte()
                });
            mcp25xxx.Write(Address.TxB0D0, data);

            // Send with TxB0 buffer.
            mcp25xxx.RequestToSend(true, false, false);
        }
        private static void ResetMcp2515(Mcp25xxx mcp25xxx)
        {
            mcp25xxx.Write(Address.Cnf1, new byte[] { 0b0000_0000 });
            mcp25xxx.Write(Address.Cnf2, new byte[] { 0b1001_0001 });
            mcp25xxx.Write(Address.Cnf3, new byte[] { 0b0000_0001 });
        }
        private static void SetMcp2515RxBufferParameters(Mcp25xxx mcp25xxx)
        {
            mcp25xxx.Write(Address.RxB0Ctrl, new byte[] { 0b0110_0000 });
            mcp25xxx.Write(Address.RxB1Ctrl, new byte[] { 0b0110_0000 });
        }
        private static void ClearRxBuffer(Mcp25xxx mcp25xxx)
        {
            mcp25xxx.Write(Address.CanIntF, new byte[] { 0b0000_0000 });
        }
        
    }
}