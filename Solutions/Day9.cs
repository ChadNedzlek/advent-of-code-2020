using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day9
    {
        public static async Task Problem1()
        {
            var data = (await Data.GetDataLines()).Select(long.Parse).ToArray();
            long targetNumber = GetTargetNumber(data);
            Console.WriteLine($"Non summing number is {targetNumber}");
        }

        public static async Task Problem2()
        {
            var data = (await Data.GetDataLines()).Select(long.Parse).ToArray();
            long targetNumber = GetTargetNumber(data);
            for (int iLine = 0; iLine < data.Length; iLine++)
            {
                long sum = 0;
                for (int iSum = iLine; iSum < data.Length; iSum++)
                {
                    sum += data[iSum];

                    if (sum == targetNumber && iSum != iLine)
                    {
                        var chunk = data.Skip(iLine).Take(iSum - iLine + 1).ToArray();
                        Console.WriteLine($"Fragment is : sum({string.Join(",", chunk)}) = {targetNumber} ");
                        long min = chunk.Min(), max = chunk.Max();
                        Console.WriteLine($"Weakness is {min} + {max} = {min + max}");
                    }

                    if (sum > targetNumber)
                        break;
                }
            }
        }

        private static long GetTargetNumber(long[] data)
        {
            List<long> nums = new List<long>();
            foreach (var value in data)
            {
                if (nums.Count == 25)
                {
                    if (!HasSum(nums, value))
                    {
                        return value;
                    }

                    nums.RemoveAt(0);
                }

                nums.Add(value);
            }

            throw new ArgumentException();
        }

        private static bool HasSum(List<long> nums, long value)
        {
            return (from i in nums from j in nums where i != j && i + j == value select i).Any();
        }
    }
}