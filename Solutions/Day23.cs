using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day23
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            List<int> cups = data[0].Select(c => c - '0').ToList();
            int currentCup = 0;
            int bigCup = cups.Max();
            for (int i = 0; i < 100; i++)
            {
                int v = cups[currentCup];
                Console.WriteLine($"-- move {i+1} --");
                Console.WriteLine($"cups : {string.Join(", ", cups.Select(c => c == v ? $"({c})" : c.ToString()))}");
                List<int> chunk = new List<int>(v);
                for (int r = 0; r < 3; r++)
                {
                    if (currentCup + 1 >= cups.Count)
                    {
                        currentCup--;
                        chunk.Add(cups[0]);
                        cups.RemoveAt(0);
                    }
                    else
                    {
                        chunk.Add(cups[currentCup + 1]);
                        cups.RemoveAt(currentCup + 1);
                    }
                }

                Console.WriteLine($"pick up: {string.Join(", ", chunk)}");

                var target = cups.IndexOf((v - 1 + bigCup) % bigCup);
                while (target == -1)
                {
                    v--;
                    if (v == 0)
                    {
                        target = cups.IndexOf(cups.Max());
                    }
                    else
                    {
                        target = cups.IndexOf((v - 1 + bigCup) % bigCup);
                    }
                }

                Console.WriteLine($"destination: {cups[target]}");
                cups.InsertRange(target + 1, chunk);
                if (target < currentCup)
                {
                    currentCup += 3;
                }

                currentCup = (currentCup + 1) % cups.Count;
                Console.WriteLine();
            }

            while (cups[0] != 1)
            {
                cups.Add(cups[0]);
                cups.RemoveAt(0);
            }
            cups.RemoveAt(0);

            Console.WriteLine($"Cups: {string.Join("", cups)}");
        }

        
        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            int cupCount = 1_000_000;
            int moveCount = 10_000_000;
            Node[] reference = new Node[cupCount + 1];
            Node currentNode = null;
            Node building = null;
            string iv = data[0];
            foreach (char c in iv)
            {
                if (currentNode == null)
                {
                    building = currentNode = new Node(c - '0');
                    reference[building.Value] = building;
                    building.Next = currentNode;
                }
                else
                {
                    building.Next = new Node(c - '0');
                    reference[building.Next.Value] = building.Next;
                    building = building.Next;
                    building.Next = currentNode;
                }
            }
            
            for (int i = iv.Length; i <= cupCount; i++)
            {
                building.Next = new Node(i);
                reference[building.Next.Value] = building.Next;
                building = building.Next;
                building.Next = currentNode;
            }


            bool Has(Node n, int target)
            {
                if (target == n.Value)
                    return true;

                if (n.Next != null)
                    return Has(n.Next, target);

                return false;
            }

            void DumpList(Node n, Node abortAt = null)
            {
                Console.Write($"{n.Value}");

                if (n.Next != abortAt && n.Next != null)
                    Console.Write(", ");

                if (n.Next != null && n.Next != abortAt)
                    DumpList(n.Next, abortAt ?? n);
            }

            bool dump = moveCount * (long)cupCount < 10_000;

            for (int i = 0; i < moveCount; i++)
            {
                if (dump) Console.Write($"-- move {i+1} -- ");
                if (i % 10_000 == 0)
                {
                    Console.Write($"{(i * 100.0 / moveCount):N2}%");
                    Console.CursorLeft = 0;
                }

                if (dump)
                {
                    Console.WriteLine();
                    Console.Write("cups: ");
                    DumpList(currentNode);
                    Console.WriteLine();
                }

                int value = currentNode.Value;
                Node chunk = currentNode.Next;
                Node skip = chunk.Next.Next.Next;

                currentNode.Next = skip;
                chunk.Next.Next.Next = null;

                if (dump)
                {
                    Console.Write("pick up: ");
                    DumpList(chunk);
                    Console.WriteLine();
                }

                int t = value;
                do
                {
                    t--;
                    if (t == 0)
                        t = cupCount;
                } while (Has(chunk, t));

                if (dump)
                {
                    Console.WriteLine($"destination: {t}");
                }

                chunk.Next.Next.Next = reference[t].Next;
                reference[t].Next = chunk;

                if (dump)
                {
                    Console.Write("after insert: ");
                    DumpList(currentNode);
                    Console.WriteLine();
                }

                currentNode = currentNode.Next;
                if (dump)
                {
                    Console.WriteLine();
                }
            }

            Console.Write("8 after 1: ");
            var print = reference[1].Next;
            for (int i = 0; i < 8; i++)
            {
                Console.Write(print.Value);
                print = print.Next;
            }
            Console.WriteLine();
            Console.WriteLine($"Product {reference[1].Next.Value} x {reference[1].Next.Next.Value} = {reference[1].Next.Value * (long)reference[1].Next.Next.Value}");
        }

        public class Node
        {
            public Node(int value)
            {
                Value = value;
            }

            public int Value { get; }
            public Node Next { get; set; }
        }
    }
}