using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace hippo
{
    class ScanResult
    {
        public DateTimeOffset WhenDidTheScanTakePlace;
        public List<string> NetworksAvailableAtMomentOfScanning;
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var scans = new List<ScanResult>();
                foreach (var scanIndex in Enumerable.Range(1, 100))
                {
                    var lines = RunProcess_GetAllReachableWLANNetworks();
                    //var lines = File.ReadAllLines("out.txt");

                    var networkNames = new List<string>();
                    for (int i = 4; i < lines.Length; i+=5)
                    {

                        // [54] SSID 11 : UPC5818082
                        // [55]     Network type            : Infrastructure
                        // [56]     Authentication          : WPA2-Personal
                        // [57]     Encryption              : CCMP
                        var match = Regex.Match(lines[i], @"SSID \d+\s+: (.+)$");
                        Console.WriteLine(lines[i]);
                        var networkName = match.Groups[1].Value;
                        Console.WriteLine(networkName);
                        networkNames.Add(networkName);
                        //lines[i]
                        //lines[i+1]
                        //lines[i+2]
                        //lines[i+3]
                    }

                    var scanResult = new ScanResult()
                    {
                        WhenDidTheScanTakePlace = DateTimeOffset.Now,
                        NetworksAvailableAtMomentOfScanning = networkNames.OrderBy(x => x).ToList()
                    };

                    Console.WriteLine(JsonConvert.SerializeObject(scanResult));

                    scans.Add(scanResult);
                    Thread.Sleep(10 * 1000);
                }
                File.WriteAllText(DateTimeOffset.UtcNow.ToString("O").Replace(":", "_") + ".txt", JsonConvert.SerializeObject(scans));
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static string[] RunProcess_GetAllReachableWLANNetworks()
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe", "/C netsh wlan show networks")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var process = Process.Start(processStartInfo);
            var readToEnd = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var lines = readToEnd.Split(new[] {"\r\n"}, StringSplitOptions.None);
            return lines;
        }
    }
}
