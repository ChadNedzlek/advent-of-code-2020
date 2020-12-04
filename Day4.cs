using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day4
    {
        public static async Task Problem1()
        {
            HashSet<string> required = new HashSet<string> {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
            HashSet<string> optional = new HashSet<string>{"cid"};
            var data = await Data.GetDataLines(4, 1);

            Dictionary<string, string> currentDataSet = new Dictionary<string, string>();

            int validCount = 0;
            void ProcessDataSet()
            {
                if (currentDataSet.Count >= required.Count && currentDataSet.Count <= (required.Count + optional.Count))
                {
                    bool valid = true;

                    foreach (var requiredKey in required)
                    {
                        if (!currentDataSet.ContainsKey(requiredKey))
                        {
                            valid = false;
                            break;
                        }

                        currentDataSet.Remove(requiredKey);
                    }

                    foreach (var optionalKey in optional)
                    {
                        currentDataSet.Remove(optionalKey);
                    }

                    if (currentDataSet.Count > 0)
                        valid = false;

                    if (valid)
                        validCount++;
                }

                currentDataSet = new Dictionary<string, string>();
            }

            foreach (var line in data)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ProcessDataSet();
                    continue;
                }

                var parts = line.Split(' ');
                foreach (var part in parts)
                {
                    var kvp = part.Split(':');
                    currentDataSet.Add(kvp[0], kvp[1]);
                }
            }

            ProcessDataSet();

            Console.WriteLine($"Found {validCount} valid passports");
        }

        public static async Task Problem2()
        {
            Dictionary<string, Predicate<string>> validation = new Dictionary<string, Predicate<string>>
            {
                {"byr", v => int.TryParse(v, out int i) && i >= 1920 && i <= 2002},
                {"iyr", v => int.TryParse(v, out int i) && i >= 2010 && i <= 2020},
                {"eyr", v => int.TryParse(v, out int i) && i >= 2020 && i <= 2030},
                {
                    "hgt",
                    v => Rx.IsM(v, @"^(\d+)(cm|in)$", out int i, out string u) &&
                        (u == "cm" && i >= 150 && i <= 193 || (u == "in" && i >= 59 && i <= 76))
                },
                {"hcl", v => Regex.IsMatch(v, @"^#[0-9a-f]{6,6}$")},
                {"ecl", v => Regex.IsMatch(v, @"^(amb|blu|brn|gry|grn|hzl|oth)$")},
                {"pid", v => Regex.IsMatch(v, @"^\d{9,9}$")},
            };

            HashSet<string> optional = new HashSet<string>{"cid"};
            var data = await Data.GetDataLines(4, 1);

            Dictionary<string, string> currentDataSet = new Dictionary<string, string>();

            int validCount = 0;
            void ProcessDataSet()
            {
                if (currentDataSet.Count >= validation.Count && currentDataSet.Count <= (validation.Count + optional.Count))
                {
                    bool valid = true;

                    foreach (var (requiredKey, rule) in validation)
                    {
                        if (!currentDataSet.TryGetValue(requiredKey, out string value) || !rule(value))
                        {
                            valid = false;
                            break;
                        }

                        currentDataSet.Remove(requiredKey);
                    }

                    foreach (var optionalKey in optional)
                    {
                        currentDataSet.Remove(optionalKey);
                    }

                    if (currentDataSet.Count > 0)
                        valid = false;

                    if (valid)
                        validCount++;
                }

                currentDataSet = new Dictionary<string, string>();
            }

            foreach (var line in data)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ProcessDataSet();
                    continue;
                }

                var parts = line.Split(' ');
                foreach (var part in parts)
                {
                    var kvp = part.Split(':');
                    currentDataSet.Add(kvp[0], kvp[1]);
                }
            }

            ProcessDataSet();

            Console.WriteLine($"Found {validCount} valid passports");
        }
    }
}