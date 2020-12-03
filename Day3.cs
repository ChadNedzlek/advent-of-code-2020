using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AdventOfCode
{
    public class Day3
    {
        public static async Task Problem1()
        {
            var lines = await Data.GetDataLines(3, 1);
            int count = 0;
            for (int iLine = 0; iLine < lines.Length; iLine++)
            {
                var line = lines[iLine];
                if (line[(iLine * 3) % line.Length] == '#')
                {
                    count++;
                }
            }
            Console.WriteLine($"Hit {count} trees");
        }

        public static async Task Problem2()
        {
            var lines = await Data.GetDataLines(3, 1);
            int count = 0;
            var slopes = new[] {(1, 1), (1, 3), (1, 5), (1, 7), (2, 1)};
            var hits = new int[slopes.Length];
            for (int iLine = 0; iLine < lines.Length; iLine++)
            {
                var line = lines[iLine];
                for (int iSlope = 0; iSlope < slopes.Length; iSlope++)
                {
                    var (down, right) = slopes[iSlope];
                    if (iLine % down == 0)
                    {
                        if (line[((iLine / down) * right) % line.Length] == '#')
                        {
                            hits[iSlope]++;
                        }
                    }
                }
            }

            Console.WriteLine($"Slope hits:");
            for (int i = 0; i < slopes.Length; i++)
            {
                var (down, right) = slopes[i];
                Console.WriteLine($"  V {down} -> {right} = {hits[i]}");
            }
            Console.WriteLine($"Total: {hits.Aggregate(1L, (x,p) => x * p)}");
        }
    }
}