using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vUtils.Network;
using vUtils.Network.Host;

namespace HCOE
{
    internal class GameServer : AsyncTcpServer
    {
        private static GameServer instance;

        public GameServer()
    : base(AddressFamily.InterNetwork, 4096, 27050)
        {
            instance = this;

            BruteforceController btc = new BruteforceController(false);

            OnClientConnect += OnClientConnected;
            OnClientDisconnect += OnClientDisconnected;
            OnClientReceive += OnMessageReceived;

            //OnClientReceive += btc.MessageHandler;
        }

        private void OnClientConnected(Client c)
        {
            Console.WriteLine("Connection received from " + c.IP);
        }

        private void OnMessageReceived(Client c, byte[] buffer)
        {
            Console.WriteLine("Message received from " + c.IP + " with " + buffer.Length + " bytes");
            Console.WriteLine("Received Hex: " + BitConverter.ToString(buffer).Replace("-", " "));

            // The game always sends an initial packet that I don't know what it means. Adding args makes the end of this packet change

            // This is a test packet
            // It does nothing, since this packet apparently has a length of 0XE24 (3620), so the server waits until the buffer is filled
            // Apparently these packets are encrypted and I don't know how its encryption works
            // I've tried investigating with IDA Pro + Hexrays but I can't read machine code and the pseucode is (apparently) broken.
            HevaWriter hw = new HevaWriter();
            hw.Write(StringToByteArray("8CF6874C9A1C"));
            c.SendMessage(hw);

            Console.WriteLine("Sent Hex: " + BitConverter.ToString(hw.GetBuffer()).Replace("-", " "));

            // ==========================================================================================================================
            // test 

            //test(c);


            hw = new HevaWriter();

            string teste = "8CF6874C9A1CAB";

            hw.Write(StringToByteArray(teste.Replace(" ", "")));
            //c.SendMessage(hw);

            // userful information

            // 1074030207 

            // initial packet
            // v3 = this->dword34 = 0x34 = 52
            // but v3 becomes 0?

            // this->pvoid3C = 0x3C = 60
            // buffer = pvoid3C + v3 = 60

            // v4 = 6
            // len = v4 - v3 = 6
        }

        private void OnClientDisconnected(Client c, string reason)
        {
            Console.WriteLine("Connection ended from " + c.IP + ". Reason: " + reason);
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
