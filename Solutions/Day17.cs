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
        private static readonly MethodInfo? s_sum = typeof(Day17).GetMethod(nameof(Sum));
        private static readonly MethodInfo? s_pad = typeof(Day17).GetMethod(nameof(Pad));
        private static readonly MethodInfo? s_zeroList = typeof(Day17).GetMethod(nameof(ZeroList));
        private static readonly MethodInfo? s_iterate = typeof(Day17).GetMethod(nameof(Iterate));
        private static readonly MethodInfo? s_count = typeof(Day17).GetMethod(nameof(Count));
        private static readonly MethodInfo? s_get = typeof(Day17).GetMethod(nameof(Get));
        private static readonly MethodInfo? s_set = typeof(Day17).GetMethod(nameof(Set));

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

            var method = s_sum.MakeGenericMethod(typeof(T).GetGenericArguments()[0]);
            return current.Sum(c => (int) method.Invoke(null, new object[] {c}));
        }

        public static List<T> Pad<T>(List<T> input)
        {
            List<T> newStuff = new List<T>(input.Count + 2);
            newStuff.Add(Zero(input[0], true));
            if (typeof(T).IsConstructedGenericType)
            {
                foreach (T chunk in input)
                {
                    newStuff.Add(
                        (T) s_pad
                            .MakeGenericMethod(typeof(T).GenericTypeArguments)
                            .Invoke(null, new object[] {chunk})
                    );
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
            
            var elementType = typeof(T).GetGenericArguments()[0];
            return (T) s_zeroList
                    .MakeGenericMethod(elementType)
                    .Invoke(null, new object[] {chunk, pad});
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

            var elementType = typeof(TInput).GetGenericArguments()[0];
            var iter = s_iterate.MakeGenericMethod(elementType);

            for (int i = 0; i < input.Count; i++)
            {
                iter.Invoke(
                    null,
                    new object[]
                    {
                        input[i], (Action<List<int>>) (l =>
                        {
                            l.Insert(0, i);
                            useCoords(l);
                        })
                    }
                );
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
                var countMethod = s_count.MakeGenericMethod(typeof(T).GetGenericArguments()[0]);
                for (int d = -1; d <= 1; d++)
                {
                    int tx = coords[index] + d;
                    if (tx < 0 || tx >= dimension.Count)
                        continue;

                    count += (int) countMethod.Invoke(
                        null,
                        new Object[] {dimension[tx], coords, index + 1, zero && d == 0}
                    );
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
                return (bool) s_get
                    .MakeGenericMethod(typeof(T).GetGenericArguments()[0])
                    .Invoke(
                        null,
                        new Object[] {dimension[x], coords, index + 1}
                    );
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
                s_set
                    .MakeGenericMethod(typeof(T).GetGenericArguments()[0])
                    .Invoke(
                        null,
                        new Object[] {dimension[x], coords, value, index + 1}
                    );
            }
        }
    }
}