using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day7
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            Dictionary<string, Dictionary<string, int>> containers = new Dictionary<string, Dictionary<string, int>>();
            foreach (var line in data)
            {
                var match = Regex.Match(line, @"^(.*) bags? contains? (?:(\d+) (.*?) bags?(?:, |\.))*$");
                if (match.Success)
                {
                    var outerBag = match.Groups[1].Value;
                    Dictionary<string, int> container = new Dictionary<string, int>();
                    for (int i = 0; i < match.Groups[2].Captures.Count; i++)
                    {
                        var count = int.Parse(match.Groups[2].Captures[i].Value);
                        var innerBag = match.Groups[3].Captures[i].Value;
                        container.Add(innerBag, count);
                    }
                    containers.Add(outerBag, container);
                }
                else
                {
                }
            }

            Dictionary<string, bool> cache = new Dictionary<string, bool>();

            bool ContainsGold(string name)
            {
                if (name == "shiny gold")
                    return true;

                if (cache.TryGetValue(name, out var cached))
                    return cached;

                if (!containers.TryGetValue(name, out var inner))
                {
                    cache.Add(name, false);
                    return false;
                }

                bool containsGold = inner.Keys.Any(ContainsGold);
                cache.Add(name, containsGold);
                return containsGold;
            }

            var totalCount = containers.Keys.Count(ContainsGold);

            Console.WriteLine($"Any bag containing gold: {totalCount-1}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            Dictionary<string, Dictionary<string, int>> containers = new Dictionary<string, Dictionary<string, int>>();
            foreach (var line in data)
            {
                var match = Regex.Match(line, @"^(.*) bags? contains? (?:(\d+) (.*?) bags?(?:, |\.))*$");
                if (match.Success)
                {
                    var outerBag = match.Groups[1].Value;
                    Dictionary<string, int> container = new Dictionary<string, int>();
                    for (int i = 0; i < match.Groups[2].Captures.Count; i++)
                    {
                        var count = int.Parse(match.Groups[2].Captures[i].Value);
                        var innerBag = match.Groups[3].Captures[i].Value;
                        container.Add(innerBag, count);
                    }
                    containers.Add(outerBag, container);
                }
                else
                {
                }
            }

            int InnerBags(string name)
            {
                if (!containers.TryGetValue(name, out var inner))
                {
                    // Only the bag itself.
                    return 1;
                }

                int total = 1;
                foreach (var (bag, count) in inner)
                {
                    total += count * InnerBags(bag);
                }

                return total;
            }
            
            Console.WriteLine($"Inside the shiny gold bag: {InnerBags("shiny gold") - 1}");
        }
    }
}