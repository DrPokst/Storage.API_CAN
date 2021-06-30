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
using Storage.API.Models;
using System.Collections;

namespace Storage.API.Services
{
    public class LedService : ILedService
    {
        public async Task<Rxmsg> SetReelLocation()
        {

            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {

                // Reset(mcp25xxx);
                mcp25xxx.Write(Address.Cnf1, new byte[] { 0b0000_0000 });
                mcp25xxx.Write(Address.Cnf2, new byte[] { 0b1001_0001 });
                mcp25xxx.Write(Address.Cnf3, new byte[] { 0b0000_0001 });

                //Rx buferio parametrai 
                mcp25xxx.Write(Address.RxB0Ctrl, new byte[] { 0b0110_0000 });
                mcp25xxx.Write(Address.RxB1Ctrl, new byte[] { 0b0110_0000 });

                //isvalau rx bufferius 
                mcp25xxx.Write(Address.CanIntF, new byte[] { 0b0000_0000 });

                //issiunciu msg, kad lauksiu rites location
                byte[] da = new byte[] { 0x00, 0x00, 0x0F, 0xF0 };
                TransmitMessage(mcp25xxx, da);


                var CANINTF = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.CanIntF)).ToArray());


                while (CANINTF[0] == false)
                {
                    CANINTF = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.CanIntF)).ToArray());
                }



                var CANINTE = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.CanIntE)).ToArray());

                byte[] data1 = mcp25xxx.ReadRxBuffer(RxBufferAddressPointer.RxB0D0, 8);
                byte[] data2 = mcp25xxx.ReadRxBuffer(RxBufferAddressPointer.RxB1D0, 8);




                byte STID0 = mcp25xxx.Read(Address.RxB0Sidh);
                byte STID1 = mcp25xxx.Read(Address.RxB0Sidl);



                //Nuskaito registrus ID paieskai ir konvertuoja i bitu masyva. 
                var bits1 = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.RxB0Sidh)).ToArray());
                var bits2 = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.RxB0Sidl)).ToArray());


                var RxB0Dlc = new BitArray(BitConverter.GetBytes(mcp25xxx.Read(Address.RxB0Dlc)).ToArray());
                RxB0Dlc[6] = false;
                RxB0Dlc[5] = false;
                RxB0Dlc[4] = false;

                int DLC = getIntFromBitArray(RxB0Dlc);


                //surasau bitus is dvieju skirtingu adresu i viena masyva
                bool[] bits3 = new bool[11] { bits2[5], bits2[6], bits2[7], bits1[0], bits1[1], bits1[2], bits1[3], bits1[4], bits1[5], bits1[6], bits1[7] };

                BitArray myBA2 = new BitArray(bits3);

                //bitu masyva pakeiciu i integer skaiciu kuris parodo atejusio CAN paketo ID 
                int ID = getIntFromBitArray(myBA2);

                Rxmsg msg = new Rxmsg
                {
                    DLC = DLC,
                    ID = ID,
                    Msg = data1
                };


                /*  Console.WriteLine("RxB0D0 pirmas bytes DEC: " + data1[0]);
                 Console.WriteLine("RxB1D0 pirmas bytes DEC: " + data2[0]);
                 Console.WriteLine("RxB1Sidh: " + STID0);
                 Console.WriteLine("RxB1Sidl: " + STID1);
                 Console.WriteLine("DLC: " + DLC);
                 Console.WriteLine("ID: " + ID);
                  */


                return msg;
            }

        }
        public async Task<bool> TakeOutReel(int id)
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {

                InitMcp();

                int tarpinis = (id / 10) + 1;
                int slotNr = id - ((tarpinis - 1) * 10);
                byte ID = Convert.ToByte(tarpinis);

                byte[] data = new byte[] { ID, (byte)slotNr, 0xF0, 0x0F, 0x00, 0x00, 0xFF, 0xFF };
                TransmitMessage(mcp25xxx, data);


            }

            return true;
        }
        public async Task<bool> TurnOnLed(int id)
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                InitMcp();

                int tarpinis = (id / 10) + 1;
                int slotNr = id - ((tarpinis - 1) * 10);
                byte ID = Convert.ToByte(tarpinis);

                byte[] data = new byte[] {ID, (byte)slotNr, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0xFF };
                TransmitMessage(mcp25xxx, data);
            }

            return true;
        }
        public async Task<bool> TurnOffLed(int id)
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                InitMcp();

                int tarpinis = (id / 10) + 1;
                int slotNr = id - ((tarpinis - 1) * 10);
                byte ID = Convert.ToByte(tarpinis);

                byte[] data = new byte[] {ID, (byte)slotNr, 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF };
                TransmitMessage(mcp25xxx, data);
            }

            return true;
        }
        public async Task<bool> TurnOnAll()
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                InitMcp();
                byte[] data = new byte[] {0, 0, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                TransmitMessage(mcp25xxx, data);
            }

            return true;
        }
        public async Task<bool> TurnOffAll()
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                InitMcp();
                byte[] data = new byte[] {0, 0, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF };
                TransmitMessage(mcp25xxx, data);
            }

            return true;
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
        private static void InitMcp()
        {
            using (Mcp25xxx mcp25xxx = GetMcp25xxxDevice())
            {
                mcp25xxx.Write(Address.Cnf1, new byte[] { 0b0000_0000 });
                mcp25xxx.Write(Address.Cnf2, new byte[] { 0b1001_0001 });
                mcp25xxx.Write(Address.Cnf3, new byte[] { 0b0000_0001 });
            }
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
        private static int getIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }
    }
}
