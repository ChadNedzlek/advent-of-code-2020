using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day14
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            Dictionary<int, long> values = new Dictionary<int, long>();
            string mask = "";
            long maskValue = 0;
            long maskMask = 0;
            foreach (var line in data)
            {
                if (Rx.IsM(line, @"^mask = ([01X]+)$", out string value))
                {
                    mask = value;
                    maskMask = Convert.ToInt64(value.Replace('0', '1').Replace('X', '0'), 2);
                    maskValue = Convert.ToInt64(value.Replace('X', '0'), 2);
                }
                else
                {
                    Rx.M(line, @"^mem\[(\d+)\] = (\d+)$", out int addr, out long v);
                    var blitted = v & ~maskMask;
                    values[addr] = blitted | maskValue;
                }
            }

            Console.WriteLine($"Ferry sum : {values.Values.Sum()}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            Dictionary<long, long> values = new Dictionary<long, long>();
            string mask = "";
            foreach (var line in data)
            {
                if (Rx.IsM(line, @"^mask = ([01X]+)$", out string value))
                {
                    mask = value;
                }
                else
                {
                    Rx.M(line, @"^mem\[(\d+)\] = (\d+)$", out long addr, out long v);

                    void WriteAll(long a, string partialMask)
                    {
                        for (int index = 0; index < mask.Length; index++)
                        {
                            var c = partialMask[index];
                            switch (c)
                            {
                                case '0':
                                    break;
                                case '1':
                                {
                                    var m = 1 << (mask.Length - index - 1);
                                    a = a | m;
                                    break;
                                }
                                case 'X':
                                {
                                    var m = 1 << (mask.Length - index - 1);
                                    StringBuilder b = new StringBuilder(partialMask);
                                    b[index] = '0';
                                    WriteAll(a | m, b.ToString());
                                    WriteAll(a & ~m, b.ToString());
                                    return;
                                }
                            }
                        }

                        values[a] = v;
                    }

                    WriteAll(addr, mask);
                }
            }

            long sum = 0;
            foreach (long value in values.Values) sum += value;
            Console.WriteLine($"Ferry sum : {sum}");
        }
    }
}