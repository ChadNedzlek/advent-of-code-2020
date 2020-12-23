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
            int cupCount = 9;
            int moveCount = 100;
            RunCupGame(data, cupCount, moveCount);
        }

        
        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            int cupCount = 1_000_000;
            int moveCount = 10_000_000;
            RunCupGame(data, cupCount, moveCount);
        }

        private static void RunCupGame(string[] data, int cupCount, int moveCount)
        {
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

            for (int i = iv.Length + 1; i <= cupCount; i++)
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

            bool dump = moveCount * (long) cupCount < 10_000;

            for (int i = 0; i < moveCount; i++)
            {
                if (dump) Console.Write($"-- move {i + 1} -- ");
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
            Console.WriteLine(
                $"Product {reference[1].Next.Value} x {reference[1].Next.Next.Value} = {reference[1].Next.Value * (long) reference[1].Next.Next.Value}"
            );
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