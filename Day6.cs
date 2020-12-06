using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day6
    {
        public static async Task Problem1()
        {
            string[] data = await Data.GetDataLines();
            int sum = 0;
            var answered = new HashSet<char>();

            void ProcessSet()
            {
                sum += answered.Count;
                answered.Clear();
            }

            foreach (string line in data)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ProcessSet();
                    continue;
                }

                foreach (char a in line)
                {
                    answered.Add(a);
                }
            }

            ProcessSet();

            Console.WriteLine($"{sum} is the sum of union group answers");
        }

        public static async Task Problem2()
        {
            string[] data = await Data.GetDataLines();
            int sum = 0;
            HashSet<char> answered = null;

            void ProcessSet()
            {
                sum += answered.Count;
                answered = null;
            }

            foreach (string line in data)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ProcessSet();
                    continue;
                }

                var set = new HashSet<char>();
                foreach (char a in line)
                {
                    set.Add(a);
                }

                if (answered == null)
                {
                    answered = set;
                }
                else
                {
                    answered.IntersectWith(set);
                }
            }

            ProcessSet();

            Console.WriteLine($"{sum} is the sum of common group answers");
        }
    }
}