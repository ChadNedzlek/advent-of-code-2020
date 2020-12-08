using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day8
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();

            RunProgram(data, out int accumulator);

            Console.WriteLine($"Accumulator: {accumulator}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();

            for (int index = 0; index < data.Length; index++)
            {
                var line = data[index];
                if (RunProgram(data, out int accumulator, index))
                {
                    Console.WriteLine($"Changed line {index}: {line}");
                    Console.WriteLine($"Accumulator: {accumulator}");
                }
            }
        }

        private static bool RunProgram(string[] data, out int result, int lineSwap = -1)
        {
            int accumulator = 0;
            HashSet<int> hit = new HashSet<int>();
            for (int ipx = 0; ipx < data.Length; ipx++)
            {
                var line = data[ipx];
                if (!hit.Add(ipx))
                {
                    result = 0;
                    return false;
                }

                Rx.M(line, @"^(...) ([+-]\d*)$", out string command, out int value);
                switch (command)
                {
                    case "nop":
                        if (lineSwap == ipx)
                        {
                            lineSwap = -1;
                            goto case "jmp";
                        }

                        break;
                    case "acc":
                        accumulator += value;
                        break;
                    case "jmp":
                        if (lineSwap == ipx)
                        {
                            lineSwap = -1;
                            goto case "nop";
                        }
                        ipx--;
                        ipx += value;
                        break;
                }
            }

            result = accumulator;
            return true;
        }
    }
}