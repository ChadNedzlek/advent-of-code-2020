using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day13
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            var start = int.Parse(data[0]);
            var busses = data[1].Split(',').Where(d => d != "x").Select(int.Parse).ToArray();
            var earliest = busses.Select(b => (b, lag: b - start % b)).OrderBy(b => b.lag).First();
            Console.WriteLine($"Bus {earliest.b} with lag {earliest.lag} = {earliest.b * earliest.lag}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            var busses = data[1].Split(',').Select((b,i) => (bus:b, index:i)).Where(b => b.bus != "x").Select((b,i) => (bus:int.Parse(b.bus), b.index, magic:i)).ToArray();
            int index = 1;
            long time = 0;
            long jump = busses[0].bus;
            while (true)
            {
                if (index == busses.Length)
                {
                    Console.WriteLine($"Magic time: {time}");
                    return;
                }

                var b = busses[index];
                long lag = (b.bus - time % b.bus) % b.bus;
                if (lag % b.bus != b.index % b.bus)
                {
                    time += jump;
                    continue;
                }

                Console.WriteLine($"Bus {b.bus} locked to lag {lag} at time {time}");
                jump *= b.bus;
                index++;
            }
        }
    }
}