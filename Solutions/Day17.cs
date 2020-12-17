using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day17
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            var initialPlane = data.Select(d => d.Select(c => c == '#').ToList()).ToList();
            List<List<List<bool>>> current = new List<List<List<bool>>> {initialPlane};
            current = RunIterations(current);
            Console.WriteLine($"Live cubes = {Sum(current)}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            var initialPlane = data.Select(d => d.Select(c => c == '#').ToList()).ToList();
            List<List<List<List<bool>>>> current = new List<List<List<List<bool>>>> {new List<List<List<bool>>> {initialPlane}};
            current = RunIterations(current);
            Console.WriteLine($"Live cubes = {Sum(current)}");
        }

        private static List<T> RunIterations<T>(List<T> current)
        {
            for (int iteration = 0; iteration < 6; iteration++)
            {
                var n = Pad(current);
                current = Pad(current);
                Iterate(
                    n,
                    coords =>
                    {
                        var count = Count(current, coords);
                        bool active = Get(current, coords);

                        bool nextActive = active ? count == 2 || count == 3 : count == 3;

                        Set(n, coords, nextActive);
                    }
                );
                current = n;
            }

            return current;
        }

        public static int Sum<T>(List<T> current)
        {
            if (typeof(T) == typeof(bool))
                return current.Count(x => (bool)(object)x);
            
            return current.Sum(c => Sum((dynamic) c));
        }

        public static List<T> Pad<T>(List<T> input)
        {
            List<T> newStuff = new List<T>(input.Count + 2);
            newStuff.Add(Zero(input[0], true));
            if (typeof(T).IsConstructedGenericType)
            {
                foreach (T chunk in input)
                {
                    newStuff.Add(Pad((dynamic) chunk));
                }
            }
            else
            {
                foreach (T chunk in input)
                {
                    newStuff.Add(chunk);
                }
            }

            newStuff.Add(Zero(input[0], true));
            return newStuff;
        }
        
        public static T Zero<T>(T chunk, bool pad)
        {
            if (typeof(T) == typeof(bool))
            {
                bool f = false;
                return Unsafe.As<bool, T>(ref f);
            }

            return ZeroList((dynamic) chunk, pad);
        }

        public static List<T> ZeroList<T>(List<T> chunk, bool pad)
        {
            var n = new List<T>(chunk.Count + (pad ? 2 : 0));
            if (pad)
            {
                n.Add(Zero(chunk[0], pad));
            }

            for (int i = 0; i < chunk.Count; i++)
            {
                n.Add(Zero(chunk[i], pad));
            }

            if (pad)
            {
                n.Add(Zero(chunk.Last(), pad));
            }

            return n;
        }

        public static void Iterate<TInput>(List<TInput> input, Action<List<int>> useCoords)
        {
            if (typeof(TInput) == typeof(bool))
            {
                for (int i = 0; i < input.Count; i++)
                {
                    useCoords(new List<int> {i});
                }

                return;
            }

            for (int i = 0; i < input.Count; i++)
            {
                Action<List<int>> recur = l =>
                {
                    l.Insert(0, i);
                    useCoords(l);
                };
                Iterate((dynamic) input[i], recur);
            }
        }

        public static int Count<T>(List<T> dimension, List<int> coords, int index = 0, bool zero = true)
        {
            int x = coords[index];
            int count = 0;
            if (index == coords.Count - 1)
            {
                for (int d = -1; d <= 1; d++)
                {
                    if (d == 0 && zero)
                    {
                        continue;
                    }

                    var tx = x + d;
                    if (tx < 0 || tx >= dimension.Count)
                        continue;

                    if ((bool) (object) dimension[tx])
                        count++;
                }
            }
            else
            {
                // This is another List of something
                for (int d = -1; d <= 1; d++)
                {
                    int tx = coords[index] + d;
                    if (tx < 0 || tx >= dimension.Count)
                        continue;

                    count += (int) Count((dynamic)dimension[tx], coords, index + 1, zero && d == 0);
                }
            }

            return count;
        }

        public static bool Get<T>(List<T> dimension, List<int> coords, int index = 0)
        {
            int x = coords[index];
            int count = 0;
            if (index == coords.Count - 1)
            {
                return (bool)(object)dimension[x];
            }
            else
            {
                return Get((dynamic) dimension[x], coords, index + 1);
            }
        }

        public static void Set<T>(List<T> dimension, List<int> coords, bool value, int index = 0)
        {
            int x = coords[index];
            int count = 0;
            if (index == coords.Count - 1)
            {
                dimension[x] = (T)(object)value;
            }
            else
            {
                Set((dynamic) dimension[x], coords, value, index + 1);
            }
        }
    }
}