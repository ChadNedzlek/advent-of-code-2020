using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Solutions
{
    public class Day24
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();

            int s = 40;
            
            bool[,] plane = new bool[s, s];

            Run(data, plane, s);

            int count = 0;
            foreach (bool x in plane)
            {
                if (x)
                    count++;
            }
            Console.WriteLine($"Flipped tiles : {count}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();

            int s = 200;
            
            bool[,] plane = new bool[s, s];

            Run(data, plane, s);

            int Get(bool[,] p, (int x, int y) c)
            {
                if (c.x < 0 || c.x >= s || c.y < 0 || c.y >= s)
                    return 0;
                return p[c.y, c.x] ? 1 : 0;
            }

            int Count(bool[,] p, int x, int y)
            {
                return Get(p, Move("e", x, y)) +
                    Get(p, Move("w", x, y)) +
                    Get(p, Move("sw", x, y)) +
                    Get(p, Move("se", x, y)) +
                    Get(p, Move("ne", x, y)) +
                    Get(p, Move("nw", x, y));
            }

            for (int i = 0; i < 100; i++)
            {
                var next = new bool[s, s];
                for (int y = 0; y < s; y++)
                {
                    for (int x = 0; x < s; x++)
                    {
                        int c = Count(plane, x, y);
                        bool b;
                        if (plane[y, x])
                        {
                            if (c == 0 || c > 2)
                                b = false;
                            else
                                b = true;
                        }
                        else
                        {
                            if (c == 2)
                                b = true;
                            else
                                b = false;
                        }

                        next[y, x] = b;
                    }
                }

                plane = next;

                int count = 0;
                foreach (bool x in plane)
                {
                    if (x)
                        count++;
                }
                Console.WriteLine($"Day {i+1} : {count}");
            }
        }

        private static void Run(string[] data, bool[,] plane, int s)
        {
            foreach (var line in data)
            {
                int c = 0;
                int x = s / 2, y = s / 2;
                for (int i = 0; i < line.Length; i++)
                {
                    string dir = line[i].ToString();
                    c++;
                    switch (Char.ToLowerInvariant(line[i]))
                    {
                        case 'e':
                        case 'w':
                            break;
                        case 'n':
                        case 's':
                            i++;
                            dir += line[i];
                            break;
                    }

                    (x, y) = Move(dir, x, y);
                }

                ref bool b = ref plane[y, x];
                b = !b;
            }
        }

        private static (int x, int y) Move(string dir, int x, int y)
        {
            switch (dir)
            {
                case "e":
                    x++;
                    break;
                case "w":
                    x--;
                    break;
                case "ne":
                    if (y % 2 == 1)
                        x++;
                    y--;
                    break;
                case "nw":
                    if (y % 2 == 0)
                        x--;
                    y--;
                    break;
                case "se":
                    if (y % 2 == 1)
                        x++;
                    y++;
                    break;
                case "sw":
                    if (y % 2 == 0)
                        x--;
                    y++;
                    break;
            }

            return (x, y);
        }

        private static void DumpGraph(int y, int x, bool[,] plane, int[,] move)
        {
            for (int py = 0; py < 20; py++)
            {
                Console.Write(py.ToString("D2"));
                if (py % 2 == 1)
                    Console.Write("  ");
                for (int px = 0; px < 20; px++)
                {
                    if (move[py, px] != 0 && !(py == y && px == x))
                    {
                        if (py == 10 && px == 10)
                        {
                            Console.Write($"-{move[py, px] % 10}-");
                        }
                        else
                        {
                            Console.Write($" {move[py, px] % 10} ");
                        }
                    }
                    else
                    {
                        if (py == 10 && px == 10)
                        {
                            if (py == y && px == x)
                            {
                                Console.Write($"[{(plane[py, px] ? "#" : ".")}]");
                            }
                            else
                            {
                                Console.Write($"-{(plane[py, px] ? "#" : ".")}-");
                            }
                        }
                        else
                        {
                            if (py == y && px == x)
                            {
                                Console.Write($"({(plane[py, px] ? "#" : ".")})");
                            }
                            else
                            {
                                Console.Write($" {(plane[py, px] ? "#" : ".")} ");
                            }
                        }
                    }

                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }

        public class Tile
        {
            public static readonly List<Tile> AllTiles = new List<Tile>();

            public bool Flipped { get; set; }

            private readonly Tile[] _tiles = new Tile[6];

            public Tile()
            {
                AllTiles.Add(this);
            }

            public Tile Get(Dir d, bool add = true)
            {
                var t = _tiles[(int) d];
                if (t == null)
                {
                    if (!add)
                        return null;

                    _tiles[(int) d] = t = new Tile();
                    t._tiles[((int)d + 3) % 6] = this;
                }

                return t;
            }
        }

        public enum Dir
        {
            E,
            SE,
            SW,
            W,
            NW,
            NE
        }
    }
}