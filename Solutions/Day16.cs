using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day16
    {
        public static async Task Problem1()
        {
            var data = ParseTicketData(await Data.GetDataLines());

            long badFields = 0;
            foreach (var ticket in data.OtherTickets)
            {
                foreach (var field in ticket)
                {
                    bool FieldValid()
                    {
                        foreach (var (_, ranges) in data.ValidRanges)
                        {
                            foreach (var (start, end) in ranges)
                            {
                                if (field >= start && field <= end)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }

                    if (!FieldValid())
                    {
                        badFields += field;
                    }
                }
            }

            Console.WriteLine($"Error rate: {badFields}");
        }

        public static async Task Problem2()
        {
            var data = ParseTicketData(await Data.GetDataLines());

            HashSet<string>[] potentialFields = new HashSet<string>[data.YourTicket.Length];
            for (int i = 0; i < potentialFields.Length; i++)
            {
                potentialFields[i] = new HashSet<string>(data.ValidRanges.Keys);
            }

            for (int iTicket = 0; iTicket < data.OtherTickets.Count; iTicket++)
            {
                var ticket = data.OtherTickets[iTicket];

                bool TicketValid()
                {
                    bool valid = true;
                    foreach (var field in ticket)
                    {
                        bool FieldValid()
                        {
                            foreach (var (_, ranges) in data.ValidRanges)
                            {
                                foreach (var (start, end) in ranges)
                                {
                                    if (field >= start && field <= end)
                                    {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        }

                        if (!FieldValid())
                        {
                            valid = false;
                            break;
                        }
                    }

                    return valid;
                }

                if (TicketValid())
                {
                    for (int index = 0; index < ticket.Length; index++)
                    {
                        int field = ticket[index];
                        List<string> toRemove = new List<string>();
                        foreach (string potential in potentialFields[index])
                        {
                            if (!data.ValidRanges[potential].Any(r => r.low <= field && r.high >= field))
                            {
                                toRemove.Add(potential);
                            }
                        }

                        foreach (var r in toRemove)
                        {
                            Console.WriteLine($"Ticket {iTicket} for field {index} has value {field}, which cannot match rule for {r}");
                            potentialFields[index].Remove(r);
                        }

                        if (toRemove.Count > 0)
                        {
                            Console.WriteLine($"  {potentialFields[index].Count} remain for field {index}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Ticket {iTicket} is invalid");
                }
            }

            bool changed;
            do
            {
                changed = false;
                for (int iField = 0; iField < potentialFields.Length; iField++)
                {
                    var identifiedField = potentialFields[iField];
                    if (identifiedField.Count == 1)
                    {
                        string name = identifiedField.First();
                        for (int iClean = 0; iClean < potentialFields.Length; iClean++)
                        {
                            var list = potentialFields[iClean];
                            if (list.Count > 1 && list.Contains(name))
                            {
                                list.Remove(name);
                                Console.WriteLine($"Because field {iField} is fixed to {name}, field {iClean} cannot be, {list.Count} remaining options");
                                changed = true;
                            }
                        }
                    }
                }
            } while (changed);

            long product = 1;
            for (int index = 0; index < data.YourTicket.Length; index++)
            {
                var field = data.YourTicket[index];

                if (potentialFields[index].Count > 1)
                {
                    throw new ArgumentException($"Could not reduce field {index}, options are {string.Join(", ", potentialFields[index])}");
                }
                
                string name = potentialFields[index].First();
                Console.Write($".. field {index} must be {name}...");

                if (name.StartsWith("departure"))
                {
                    product *= field;
                    Console.WriteLine(" counting");
                }
                else
                {
                    Console.WriteLine(" ignoring");
                }
            }

            Console.WriteLine($"Ticket mul: {product}");
        }

        private static TicketData ParseTicketData(string[] data)
        {
            bool finishedRules = false;
            bool finishedTicket = false;
            TicketData ticket = new TicketData();
            foreach (var line in data)
            {
                if (!finishedRules)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        finishedRules = true;
                        continue;
                    }

                    Rx.M(line, @"^(.*): (\d+)-(\d+) or (\d+)-(\d+)$", out string name, out int start1, out int end1, out int start2, out int end2);
                    if (!ticket.ValidRanges.TryGetValue(name, out var ranges))
                    {
                        ticket.ValidRanges.Add(name, ranges = new List<(int low, int high)>());
                    }
                    ranges.Add((start1, end1));
                    ranges.Add((start2, end2));
                    continue;
                }

                if (!finishedTicket)
                {
                    if (line == "your ticket:")
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        finishedTicket = true;
                        continue;
                    }

                    ticket.YourTicket = line.Split(',').Select(int.Parse).ToArray();
                    continue;
                }

                if (line == "nearby tickets:")
                {
                    continue;
                }

                ticket.OtherTickets.Add(line.Split(',').Select(int.Parse).ToArray());
            }

            return ticket;
        }

        public class TicketData
        {
            public Dictionary<string, List<(int low, int high)>> ValidRanges { get; } = new Dictionary<string, List<(int low, int high)>>();

            public int[] YourTicket { get; set; }

            public List<int[]> OtherTickets { get; set; } = new List<int[]>();
        }
    }
}