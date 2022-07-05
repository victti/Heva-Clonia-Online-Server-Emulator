using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using vUtils.Network.Host;

namespace HCOE
{
    internal class BruteforceController
    {
        private bool autoStartP;

        private List<int> bytes;

        private bool running;

        private System.Timers.Timer timer;

        private DateTime lastSentTime;

        public BruteforceController(bool autoStartP)
        {
            this.autoStartP = autoStartP;

            bytes = new List<int>();
            bytes.Add(28);
            bytes.Add(28);
            bytes.Add(28);
            bytes.Add(28);
            bytes.Add(28);
            bytes.Add(28);
            bytes.Add(28);

            bytes[1] = 62;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) => HandleTimer();
            timer.Start();

            if (autoStartP)
            {
                StartHeva();
                lastSentTime = DateTime.Now;
            }
        }

        private void HandleTimer()
        {
            if (autoStartP && running && DateTime.Now.Subtract(lastSentTime).TotalSeconds > 20)
            {
                lastSentTime = DateTime.Now;
                StartHeva();
            }
        }

        private void StartHeva()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(@"D:\Games\Heva Clonia\HevaUS.exe");
            startInfo.Arguments = "127.0.0.1 27050";
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = @"D:\Games\Heva Clonia\";
            Process.Start(startInfo);
        }

        public void MessageHandler(Client c, byte[] buffer)
        {
            _ = AsyncMessageHandler(c, buffer);
        }

        private async Task AsyncMessageHandler(Client c, byte[] buffer)
        {
            HevaWriter hw;

            running = true;

            Console.WriteLine(BitConverter.ToString(buffer));
            if (!BitConverter.ToString(buffer).Contains("8C-F6-6E-29-16-63-A9-BD-5A-42-52-68-C8-29"))
                running = false;

            Random r = new Random();

            for (int i = 0; i < 10000; i++)
            {
                if (!c.Connected || !running)
                {
                    if (!running)
                    {
                        Console.WriteLine("Stopped due to different message received");
                        break;
                    }

                    if (autoStartP)
                    {
                        await Task.Delay(1000);

                        StartHeva();
                        Console.WriteLine("Stopped due to game crash, restarting");
                    }
                    break;
                }


                hw = new HevaWriter();

                //for (int j = 0; j < bytes.Count; j++)
                //    hw.Write((byte)bytes[j]);

                byte[] bytesA = new byte[25];
                r.NextBytes(bytesA);

                hw.Write(bytesA);

                c.SendMessage(hw);
                lastSentTime = DateTime.Now;
                await Task.Delay(20);
                Console.WriteLine("Sent Hex: " + BitConverter.ToString(hw.GetBuffer()).Replace("-", " "));

                bytes[0] += 1;

                for (int j = 0; j < bytes.Count; j++)
                {
                    if (bytes[j] == 256)
                    {
                        bytes[j] = 0;

                        if (j < bytes.Count - 1)
                        {
                            bytes[j + 1] += 1;
                        }
                        else
                        {
                            bytes.Add(0);
                        }
                    }
                }
            }
        }
    }
}
