using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var infoForAllNetworks = File.ReadAllText("infoForMultipleNetworks.txt");

            var strings = infoForAllNetworks
                .Split(new[] {"\r\n\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1);

            foreach (var s in strings)
            {
                Console.WriteLine("========================");
                Console.WriteLine(s);
                Console.WriteLine("------------------------");
                var networkScanResult = ClassX.Parse(s);
                Console.WriteLine(networkScanResult);
                Console.WriteLine("========================");

            }

//            var infoForOneNetwork = File.ReadAllText("infoForOneNetwork.txt");
//            var networkScanResult = ClassX.Parse(infoForOneNetwork);
//            Console.WriteLine(networkScanResult);
            
            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
    }

    public class ClassX
    {
        public static NetworkScanResult Parse(string infoToBeParsed)
        {
            var lines = infoToBeParsed.Split(new[] {"\r\n"}, StringSplitOptions.None);
            var match = Regex.Match(lines[0], @"SSID \d+\s+: (.+)$");
            var networkName = match.Groups[1].Value;
            //         Signal             : 99%
            var match2 = Regex.Match(lines[5], @"\s+Signal\s+:\s+(\d+)%$");
            var signalStrength = int.Parse(match2.Groups[1].Value);

            return new NetworkScanResult
            {
                WhenDidTheScanTakePlace = DateTimeOffset.UtcNow,
                NetworkName = networkName,
                SignalStrength = signalStrength
            };
        }
    }

    public class NetworkScanResult
    {
        public DateTimeOffset WhenDidTheScanTakePlace { get; set; }
        public string NetworkName { get; set; }
        public int SignalStrength { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
