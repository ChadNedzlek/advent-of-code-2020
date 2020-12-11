using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day10
    {
        public static async Task Problem1()
        {
            var data = (await Data.GetDataLines()).Select(int.Parse).Append(0).OrderBy(x => x).ToArray();
            int[] diffs = new int[4];
            diffs[3]++;
            for (int i = 0; i < data.Length - 1; i++)
            {
                diffs[data[i + 1] - data[i]]++;
            }

            Console.WriteLine($"Diffs {string.Join(",", diffs)} : {diffs[1]} * {diffs[3]} = {diffs[1] * diffs[3]}");
        }
        
        public static async Task Problem2()
        {
            var data = (await Data.GetDataLines()).Select(int.Parse).ToHashSet();
            int magic = data.Max() + 3;
            Dictionary<int, long> cache = new Dictionary<int, long> {{0, 1}};

            long WaysTo(int target)
            {
                if (cache.TryGetValue(target, out long waysTo))
                {
                    return waysTo;
                }

                if (!data.Contains(target) && target != magic)
                    return 0;

                long valid = 0;
                for (int i = 1; i < 4; i++)
                {
                    valid += WaysTo(target - i);
                }

                cache.Add(target, valid);
                return valid;
            }
            Console.WriteLine($"Valid arrangements: {WaysTo(magic)}");
        }

        // Problem 2, no recursion
        public static async Task Problem3()
        {
            var data = (await Data.GetDataLines()).Select(int.Parse).ToHashSet();
            int magic = data.Max() + 3;
            long[] paths = new long[magic+4];
            paths[0] = 1;

            for (int i = 0; i <= magic; i++)
            {
                for (int j = i + 1; j <= i + 3; j++)
                {
                    if (data.Contains(j) || j == magic)
                    {
                        paths[j] += paths[i];
                    }
                }
            }
            
            Console.WriteLine($"Valid arrangements: {paths[magic]:X}");
        }
    }
}