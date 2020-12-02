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
                (string minString, string maxString, string letterString, string password) =
                    Regex.Match(line, @"^(\d+)-(\d+) (.): (.*)$");
                int min = int.Parse(minString);
                int max = int.Parse(maxString);
                char letter = letterString[0];
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
                (string aString, string bString, string letterString, string password) =
                    Regex.Match(line, @"^(\d+)-(\d+) (.): (.*)$");
                int a = int.Parse(aString);
                int b = int.Parse(bString);
                char letter = letterString[0];
                if ((password[a - 1] == letter) ^ (password[b - 1] == letter))
                {
                    valid++;
                }
            }

            Console.WriteLine($"Found {valid} valid passwords");
        }
    }
}