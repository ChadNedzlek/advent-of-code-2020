using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day15
    {
        public static async Task Problem1()
        {
            Go(2020, await Data.GetDataLines());
        }
        public static async Task Problem2()
        {
            Go(30000000, await Data.GetDataLines());
        }

        private static void Go(int target, string[] data)
        {
            Dictionary<long, long> prev = new Dictionary<long, long>();
            Dictionary<long, long> twoPrev = new Dictionary<long, long>();
            int[] ints = data[0].Split(',').Select(int.Parse).ToArray();
            for (int index = 0; index < ints.Length; index++)
            {
                prev[ints[index]] = index;
            }

            long lastSpoken = ints.Last();
            for (long i = prev.Count;; i++)
            {
                long toSpeak;
                if (twoPrev.TryGetValue(lastSpoken, out var tpVal))
                {
                    toSpeak = prev[lastSpoken] - tpVal;
                }
                else
                {
                    toSpeak = 0;
                }

                if (prev.TryGetValue(toSpeak, out var pVal))
                {
                    twoPrev[toSpeak] = pVal;
                }

                prev[toSpeak] = i;
                lastSpoken = toSpeak;

                if (i == target - 1)
                {
                    Console.WriteLine($"Pointless math: {toSpeak}");
                    return;
                }
            }
        }
    }
}