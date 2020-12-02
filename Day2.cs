using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day2
    {
        public static async Task Problem1()
        {
            string[] data = await Data.GetDataLines(2, 1);
            int valid = 0;
            foreach (string line in data)
            {
                Rx.M(line, @"^(\d+)-(\d+) (.): (.*)$", out int min, out int max, out char letter, out string password);

                int letterCount = password.Count(c => c == letter);
                if (letterCount >= min && letterCount <= max)
                {
                    valid++;
                }
            }

            Console.WriteLine($"Found {valid} valid passwords");
        }

        public static async Task Problem2()
        {
            string[] data = await Data.GetDataLines(2, 1);
            int valid = 0;
            foreach (string line in data)
            {
                Rx.M(line, @"^(\d+)-(\d+) (.): (.*)$", out int a, out int b, out char letter, out string password);

                if ((password[a - 1] == letter) ^ (password[b - 1] == letter))
                {
                    valid++;
                }
            }

            Console.WriteLine($"Found {valid} valid passwords");
        }
    }
}