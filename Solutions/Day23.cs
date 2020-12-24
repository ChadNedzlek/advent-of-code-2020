using System;
using System.Diagnostics;
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
            RunSimpleCupGame(data, cupCount, moveCount);
        }

        
        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            int cupCount = 1_000_000;
            int moveCount = 10_000_000;
            Stopwatch collapsed = Stopwatch.StartNew();
            //RunCollapsedCupGame(data, cupCount, moveCount);
            collapsed.Stop();
            Stopwatch simple = Stopwatch.StartNew();
            RunSimpleCupGame(data, cupCount, moveCount);
            simple.Stop();

            Console.WriteLine($"Comparison: simple = {simple.Elapsed:g}, collapsed = {collapsed.Elapsed:g}");
        }
        
        private static void RunSimpleCupGame(string[] data, int cupCount, int moveCount)
        {
            SimpleNode[] reference = new SimpleNode[cupCount + 1];
            SimpleNode currentNode = null;
            SimpleNode building = null;
            string iv = data[0];
            foreach (char c in iv)
            {
                if (currentNode == null)
                {
                    building = currentNode = new SimpleNode(c - '0');
                    reference[building.Value] = building;
                    building.Next = currentNode;
                }
                else
                {
                    building.Next = new SimpleNode(c - '0');
                    reference[building.Next.Value] = building.Next;
                    building = building.Next;
                    building.Next = currentNode;
                }
            }

            for (int i = iv.Length + 1; i <= cupCount; i++)
            {
                building.Next = new SimpleNode(i);
                reference[building.Next.Value] = building.Next;
                building = building.Next;
                building.Next = currentNode;
            }


            bool Has(SimpleNode n, int target)
            {
                if (target == n.Value)
                    return true;

                if (n.Next != null)
                    return Has(n.Next, target);

                return false;
            }

            for (int i = 0; i < moveCount; i++)
            {
                int value = currentNode.Value;
                SimpleNode chunk = currentNode.Next;
                SimpleNode skip = chunk.Skip(3);

                currentNode.Next = skip;
                chunk.Skip(2).Next = null;

                int t = value;
                do
                {
                    t--;
                    if (t == 0)
                        t = cupCount;
                } while (Has(chunk, t));

                chunk.Next.Next.Next = reference[t].Next;
                reference[t].Next = chunk;

                currentNode = currentNode.Next;
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

        private static void RunCollapsedCupGame(string[] data, int cupCount, int moveCount)
        {
            CollapsedNode head = null;
            CollapsedNode tail = null;
            string iv = data[0];
            foreach (char c in iv)
            {
                var building = new CollapsedNode(c - '0');
                if (head == null)
                {
                    tail = head = building;
                }
                tail.Connect(building);
                building.Connect(head);
                tail = building;
            }

            var remNode = new CollapsedNode(iv.Length + 1, cupCount);
            tail.Connect(remNode);
            remNode.Connect(head);

            bool Has(CollapsedNode n, int target)
            {
                if (target >= n.Low && target <= n.High)
                    return true;

                if (n.HasNext)
                    return Has(n.Next, target);

                return false;
            }

            CollapsedNode Find(CollapsedNode h, int label)
            {
                while (true)
                {
                    if (label >= h.Low && label <= h.High) return h;

                    h = h.Next;
                }
            }

            for (int i = 0; i < moveCount; i++)
            {
                int value = head.Low;
                var chunk = head.Skip(1);
                var skip = chunk.Skip(3);
                CollapsedNode chunkTail = chunk.Skip(2, end: true);
                head.Connect(null);
                chunkTail.Connect(null);
                head.Connect(skip);
                
                int t = value;
                do
                {
                    t--;
                    if (t == 0)
                        t = cupCount;
                } while (Has(chunk, t));

                CollapsedNode targetNode = Find(head, t);
                CollapsedNode afterTarget = targetNode.Skip(t - targetNode.Low + 1);
                chunkTail.Connect(afterTarget);
                targetNode.Connect(chunk);

                head = head.Skip(1);
            }

            Console.Write("8 after 1: ");
            var one = Find(head, 1);
            var print = one.Skip(1);
            for (int i = 0; i < 8; i++)
            {
                Console.Write(print.Low);
                print = print.Skip(1);
            }

            Console.WriteLine();
            Console.WriteLine(
                $"Product {one.Skip(1).Low} x {one.Skip(2).Low} = {one.Skip(1).Low * (long) one.Skip(2).Low}"
            );
        }

        public class SimpleNode
        {
            public SimpleNode(int value)
            {
                Value = value;
            }

            public int Value { get; }
            public SimpleNode Next { get; set; }

            public SimpleNode Skip(int skip) => skip == 0 ? this : Next.Skip(skip - 1);
        }

        public class CollapsedNode
        {
            public CollapsedNode(int value)
            {
                Low = High = value;
            }

            public CollapsedNode(int low, int high)
            {
                Low = low;
                High = high;
            }

            public int Low { get; private set; }
            public int High { get; private set; }

            public CollapsedNode Next { get; private set; }
            public bool HasNext => Next != null;

            public bool IsSingle => Low == High;

            public CollapsedNode Skip(int skip, bool end = false)
            {
                if (skip == 0)
                {
                    return this;
                }

                if (skip >= Width)
                {
                    // we need to skip this whole node
                    return Next.Skip(skip - Width);
                }

                if (end)
                {
                    Split(Low + skip + 1, High);
                    return this;
                }

                Split(Low + skip, High);

                return Next;
            }

            private int Width => High - Low + 1;

            private void Split(int low, int high)
            {
                if (low == Low)
                {
                    if (high == High)
                        return;

                    var n = new CollapsedNode(High + 1, high);
                    High = high;
                    n.Connect(Next);
                    Connect(n);
                    return;
                }

                if (high == High)
                {
                    var n = new CollapsedNode(low, High);
                    High = low - 1;
                    n.Connect(Next);
                    Connect(n);
                    return;
                }

                var midNode = new CollapsedNode(low, high);
                var highNode  = new CollapsedNode(high +1, High);
                High = low - 1;
                highNode.Connect(Next);
                midNode.Connect(highNode);
                Connect(midNode);
            }

            public void Connect(CollapsedNode n)
            {
                if (n != null && n.Low == n.High + 1)
                {
                    // We can combine this node
                    High = n.High;
                    return;
                }

                Next = n;
            }

            public override string ToString()
            {
                return String.Concat(IsSingle ? Low.ToString() : $"{Low}-{High}", " ", HasNext ? "=>" : "=X");
            }
        }
    }
}