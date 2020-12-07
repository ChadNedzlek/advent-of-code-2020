using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day1
    {
        public static async Task Problem1()
        {
            HashSet<int> value = new HashSet<int>();
            foreach (string line in await Data.GetDataLines())
            {
                var number = int.Parse(line);
                int needed = 2020 - number;
                if (value.Contains(needed))
                {
                    Console.WriteLine($"Found:  {number} * {needed} = {number * needed}");
                    return;
                }

                value.Add(number);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to locate answer");
            Console.ResetColor();
        }

        public static async Task Problem2()
        {
            HashSet<int> seen = new HashSet<int>();
            Dictionary<int, (int, int)> pairs = new Dictionary<int, (int, int)>();
            foreach (string line in await Data.GetDataLines())
            {
                var number = int.Parse(line);
                int needed = 2020 - number;
                if (pairs.TryGetValue(needed, out var pair))
                {
                    Console.WriteLine(
                        $"Found: {pair.Item1} * {pair.Item2} * {number} = {number * pair.Item1 * pair.Item2}"
                    );
                    return;
                }

                foreach (var a in seen)
                {
                    var partialSum = a + number;
                    if (partialSum < 2020)
                    {
                        pairs.TryAdd(a + number, (a, number));
                    }
                }

                seen.Add(number);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to locate answer");
            Console.ResetColor();
        }
    }
}