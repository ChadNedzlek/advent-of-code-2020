using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Irony.Parsing;

namespace AdventOfCode.Solutions
{
    public class Day20
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();

            Dictionary<int, char[,]> tiles = new Dictionary<int, char[,]>();
            for (int i = 0; i < data.Length; i++)
            {
                var line = data[i];
                Rx.M(line, @"Tile (\d+):", out int id);
                i++;
                var row = data[i];
                char[,] tile = new char[row.Length, row.Length];
                for (int r = 0; r < row.Length; r++)
                {
                    row = data[i + r];
                    for (int c = 0; c < row.Length; c++)
                    {
                        tile[r, c] = row[c] == '#' ? '1' : '0';
                    }
                }

                tiles.Add(id, tile);
                i += row.Length;
            }

            int[] CalcSignatures(char[,] t)
            {
                string top = "", bottom = "", left = "", right = "";
                int end = t.GetUpperBound(0);
                for (int i = 0; i <= end; i++)
                {
                    top += t[0, i];
                    bottom += t[end, i];
                    left += t[i, 0];
                    right += t[i, end];
                }

                return new[]
                {
                    Math.Min(Convert.ToInt32(top, 2), Convert.ToInt32(new string(top.Reverse().ToArray()), 2)),
                    Math.Min(Convert.ToInt32(bottom, 2), Convert.ToInt32(new string(bottom.Reverse().ToArray()), 2)),
                    Math.Min(Convert.ToInt32(left, 2), Convert.ToInt32(new string(left.Reverse().ToArray()), 2)),
                    Math.Min(Convert.ToInt32(right, 2), Convert.ToInt32(new string(right.Reverse().ToArray()), 2)),
                };
            }

            Dictionary<int, int[]> signatures = tiles.ToDictionary(t => t.Key, t => CalcSignatures(t.Value));
            Dictionary<int, int> edge = new Dictionary<int, int>();
            foreach (var (_, sigs) in signatures)
            {
                foreach (var sig in sigs)
                    if (edge.TryGetValue(sig, out var c))
                    {
                        edge[sig] = c + 1;
                    }
                    else
                    {
                        edge.Add(sig, 1);
                    }
            }

            var doubleUnique = signatures.Where(s => s.Value.Count(n => edge[n] == 1) == 2).ToArray();
            var mult = doubleUnique.Select(s => s.Key).Aggregate(1L, (s, a) => s * a);
            Console.WriteLine($"Corner Mult: {mult}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();

            List<Tile> tiles = new List<Tile>();
            int size = 0;
            for (int i = 0; i < data.Length; i++)
            {
                var line = data[i];
                Rx.M(line, @"Tile (\d+):", out int id);
                i++;
                var row = data[i];
                size = row.Length;
                char[,] tile = new char[row.Length, row.Length];
                for (int r = 0; r < row.Length; r++)
                {
                    row = data[i + r];
                    for (int c = 0; c < row.Length; c++)
                    {
                        tile[r, c] = row[c] == '#' ? '1' : '0';
                    }
                }

                tiles.Add(new Tile(id, tile));
                i += row.Length;
            }
            Dictionary<int, int> edge = new Dictionary<int, int>();
            foreach (var t in tiles)
            {
                foreach (var sig in t.Sigs)
                    if (edge.TryGetValue(sig, out var c))
                    {
                        edge[sig] = c + 1;
                    }
                    else
                    {
                        edge.Add(sig, 1);
                    }
            }

            bool IsEdge(int sig)
            {
                return edge[sig] == 1;
            }

            var topLeft = tiles.FirstOrDefault(s => s.Sigs.Count(IsEdge) == 2);
            while (!IsEdge(topLeft.Sigs[Top]) || !IsEdge(topLeft.Sigs[Left]))
            {
                topLeft.RotateRight();
            }

            int dim = Convert.ToInt32(Math.Sqrt(tiles.Count));
            Tile[,] placed = new Tile[dim,dim];
            placed[0, 0] = topLeft;
            for (int r = 0; r < dim; r++)
            {
                for (int c = 0; c < dim; c++)
                {
                    if (placed[r, c] != null)
                    {
                        continue;
                    }

                    if (c > 0)
                    {
                        var left = placed[r, c-1];
                        var placing = tiles.Single(t => t != left && t.Sigs.Contains(left.Sigs[Right]));

                        while (placing.Sigs[Left] != left.Sigs[Right])
                        {
                            placing.RotateRight();
                        }

                        bool needFlip = false;
                        for (int i = 0; i < size; i++)
                        {
                            if (left.Cells[i, size - 1] != placing.Cells[i, 0])
                            {
                                needFlip = true;
                                break;
                            }
                        }

                        if (needFlip)
                            placing.FlipVertical();

                        placed[r, c] = placing;
                        continue;
                    }

                    if (r > 0)
                    {
                        var above = placed[r - 1, c];
                        var placing = tiles.Single(t => t != above && t.Sigs.Contains(above.Sigs[Bottom]));

                        while (placing.Sigs[Top] != above.Sigs[Bottom])
                        {
                            placing.RotateRight();
                        }
                        bool needFlip = false;
                        for (int i = 0; i < size; i++)
                        {
                            if (above.Cells[size - 1, i] != placing.Cells[0, i])
                            {
                                needFlip = true;
                                break;
                            }
                        }

                        if (needFlip)
                            placing.FlipHorizontal();
                        placed[r, c] = placing;
                        continue;
                    }
                }
            }
            
            int aSize = dim * (size - 2);
            char[,] assembled = new char[aSize,aSize];
            for (int tr = 0; tr < dim; tr++)
            {
                for (int tc = 0; tc < dim; tc++)
                {
                    for (int cr = 1; cr < size - 1; cr++)
                    {
                        for (int cc = 1; cc < size - 1; cc++)
                        {
                            assembled[tr * (size - 2) + cr - 1, tc * (size - 2) + cc - 1] =
                                placed[tr, tc].Cells[cr, cc];
                        }
                    }
                }
            }

            bool IsMonsterAt(char[,]a, int r, int c)
            {
                return a[r, c + 18] == '1' &&
                    a[r + 1, c] == '1' &&
                    a[r + 1, c + 5] == '1' &&
                    a[r + 1, c + 6] == '1' &&
                    a[r + 1, c + 11] == '1' &&
                    a[r + 1, c + 12] == '1' &&
                    a[r + 1, c + 18] == '1' &&
                    a[r + 1, c + 17] == '1' &&
                    a[r + 1, c + 19] == '1' &&
                    a[r + 2, c + 1] == '1' &&
                    a[r + 2, c + 4] == '1' &&
                    a[r + 2, c + 7] == '1' &&
                    a[r + 2, c + 10] == '1' &&
                    a[r + 2, c + 13] == '1' &&
                    a[r + 2, c + 16] == '1';
            }

            int MonsterCount(char[,] a)
            {
                int count = 0;
                for (int r = 0; r < aSize - 2; r++)
                {
                    for (int c = 0; c < aSize - 19; c++)
                    {
                        if (IsMonsterAt(a, r, c))
                        {
                            count++;
                        }
                    }
                }

                return count;
            }


            List<char[,]> permutations = new List<char[,]>
            {
                assembled,
                RotateRight(assembled),
                RotateRight(RotateRight(assembled)),
                RotateRight(RotateRight(RotateRight(assembled))),
                FlipHorizontal(assembled),
                RotateRight(FlipHorizontal(assembled)),
                RotateRight(RotateRight(FlipHorizontal(assembled))),
                RotateRight(RotateRight(RotateRight(FlipHorizontal(assembled)))),
            };

            var oriented = permutations.Single(p => MonsterCount(p) > 0);
            
            char[,] calmed = new char[aSize,aSize];
            Array.Copy(oriented, calmed, oriented.Length);
            for (int r = 0; r < aSize - 2; r++)
            {
                for (int c = 0; c < aSize - 19; c++)
                {
                    if (IsMonsterAt(oriented, r, c))
                    {
                        calmed[r, c + 18] = '0';
                        calmed[r + 1, c] = '0';
                        calmed[r + 1, c + 5] = '0';
                        calmed[r + 1, c + 6] = '0';
                        calmed[r + 1, c + 11] = '0';
                        calmed[r + 1, c + 12] = '0';
                        calmed[r + 1, c + 18] = '0';
                        calmed[r + 1, c + 17] = '0';
                        calmed[r + 1, c + 19] = '0';
                        calmed[r + 2, c + 1] = '0';
                        calmed[r + 2, c + 4] = '0';
                        calmed[r + 2, c + 7] = '0';
                        calmed[r + 2, c + 10] = '0';
                        calmed[r + 2, c + 13] = '0';
                        calmed[r + 2, c + 16] = '0';
                    }
                }
            }

            int roughness = 0;
            foreach (char cell in calmed)
            {
                if (cell == '1')
                    roughness++;
            }
            Console.WriteLine($"Roughness: {roughness}");
        }

        public class Tile
        {
            public Tile(int id, char[,] cells)
            {
                Id = id;
                Cells = cells;
                Sigs = CalcSignatures(Cells);
            }

            public int Id { get; }
            public char[,] Cells { get; private set; }
            public int[] Sigs { get; private set; }

            public void RotateRight()
            {
                Cells = Day20.RotateRight(Cells);
                Sigs = new[] {Sigs[Left], Sigs[Top], Sigs[Right], Sigs[Bottom]};
            }

            public void FlipVertical()
            {
                Cells = Day20.FlipVertical(Cells);
                Sigs = new[] {Sigs[Bottom], Sigs[Right], Sigs[Top], Sigs[Left]};
            }

            public void FlipHorizontal()
            {
                Cells = Day20.FlipHorizontal(Cells);
                Sigs = new[] {Sigs[Top], Sigs[Left], Sigs[Bottom], Sigs[Right]};
            }
        }

        public static int Top = 0;
        public static int Right = 1;
        public static int Bottom = 2;
        public static int Left = 3;

        public static int[] CalcSignatures(char[,] t)
        {
            string top = "", bottom = "", left = "", right = "";
            int end = t.GetUpperBound(0);
            for (int i = 0; i <= end; i++)
            {
                top += t[0, i];
                bottom += t[end, i];
                left += t[i, 0];
                right += t[i, end];
            }

            return new[]
            {
                Math.Min(Convert.ToInt32(top, 2), Convert.ToInt32(new string(top.Reverse().ToArray()), 2)),
                Math.Min(Convert.ToInt32(right, 2), Convert.ToInt32(new string(right.Reverse().ToArray()), 2)),
                Math.Min(Convert.ToInt32(bottom, 2), Convert.ToInt32(new string(bottom.Reverse().ToArray()), 2)),
                Math.Min(Convert.ToInt32(left, 2), Convert.ToInt32(new string(left.Reverse().ToArray()), 2)),
            };
        }

        public static char[,] RotateRight(char[,] a)
        {
            int end = a.GetUpperBound(0);
            char[,] result = new char[end + 1, end + 1];
            for (int r = 0; r <= end; r++)
            {
                for (int c = 0; c <= end; c++)
                {
                    result[c, end - r] = a[r, c];
                }
            }

            return result;
        }

        public static char[,] FlipHorizontal(char[,] a)
        {
            int end = a.GetUpperBound(0);
            char[,] result = new char[end + 1, end + 1];
            for (int r = 0; r <= end; r++)
            {
                for (int c = 0; c <= end; c++)
                {
                    result[r, end - c] = a[r, c];
                }
            }

            return result;
        }

        public static char[,] FlipVertical(char[,] a)
        {
            int end = a.GetUpperBound(0);
            char[,] result = new char[end + 1, end + 1];
            for (int r = 0; r <= end; r++)
            {
                for (int c = 0; c <= end; c++)
                {
                    result[end - r, c] = a[r, c];
                }
            }

            return result;
        }
    }
}