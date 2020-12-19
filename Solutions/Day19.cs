using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day19
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            Dictionary<int, string> text = new Dictionary<int, string>();
            var rules = data.TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
            foreach (var rule in rules)
            {
                var parts = rule.Split(':');
                int num = int.Parse(parts[0]);
                text.Add(num, parts[1].Trim());
            }

            var messages = data.SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).ToArray();

            int count = 0;
            foreach (var m in messages)
            {
                var matches = IsMatch(text, m, 0, 0);
                if (matches.Count(l => m.Length == l) > 0)
                {
                    Console.WriteLine($"s    : {m}");
                    count++;
                }
                else
                {
                    Console.WriteLine($"ERROR: {m}");
                }
            }

            Console.WriteLine($"Recursy way: {count}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            Dictionary<int, string> text = new Dictionary<int, string>();
            var rules = data.TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
            foreach (var rule in rules)
            {
                var parts = rule.Split(':');
                int num = int.Parse(parts[0]);
                text.Add(num, parts[1].Trim());
            }

            text[8] = "42 | 42 8";
            text[11] = "42 31 | 42 11 31";
            
            var messages = data.SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).ToArray();

            int count = 0;

            foreach (var m in messages)
            {
                var matches = IsMatch(text, m, 0, 0);
                if (matches.Count(l => m.Length == l) > 0)
                {
                    Console.WriteLine($"s    : {m}");
                    count++;
                }
                else
                {
                    Console.WriteLine($"ERROR: {m}");
                }
            }

            Console.WriteLine($"Recursy way: {count}");
        }

        static IList<int> IsMatch(Dictionary<int, string> rules, string input, int rule, int index)
        {
            if (index >= input.Length)
            {
                return Array.Empty<int>();
            }

            string text = rules[rule];
            if (text[0] == '"')
            {
                if (input[index] == text[1])
                {
                    return new []{index + 1};
                }

                return Array.Empty<int>();
            }

            List<int> options = new List<int>();
            foreach (var opt in text.Split('|'))
            {
                options.AddRange(
                    MatchesSequence(rules, input, index, opt.Trim().Split(' ').Select(int.Parse).ToArray(), 0)
                );
            }

            return options;
        }

        static IList<int> MatchesSequence(Dictionary<int, string> rules, string input, int index, int[] ids, int iId)
        {
            var firstPart = IsMatch(rules, input, ids[iId], index);

            if (iId == ids.Length - 1)
                return firstPart;
            
            List<int> consumed = new List<int>();
            foreach (var part in firstPart)
            {
                consumed.AddRange(MatchesSequence(rules, input, part, ids, iId + 1));
            }

            return consumed;
        }
    }
}