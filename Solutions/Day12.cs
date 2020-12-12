using System;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day12
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            int x = 0, y = 0, dx = 1, dy = 0;
            foreach (var line in data)
            {
                int amount = int.Parse(line.Substring(1));
                switch (line[0])
                {
                    case 'N':
                        y += amount;
                        break;
                    case 'E':
                        x += amount;
                        break;
                    case 'S':
                        y -= amount;
                        break;
                    case 'W':
                        x -= amount;
                        break;
                    case 'F':
                        x += amount * dx;
                        y += amount * dy;
                        break;
                    case 'L':
                    {
                        for (int i = 0; i < amount; i += 90)
                        {
                            var o = dx;
                            dx = -dy;
                            dy = o;
                        }

                        break;
                    }
                    case 'R':
                    {
                        for (int i = 0; i < amount; i += 90)
                        {
                            var o = dx;
                            dx = dy;
                            dy = -o;
                        }
                    }
                        break;
                }
            }

            Console.WriteLine($"abs({x}) + abs({y}) = {Math.Abs(x) + Math.Abs(y)}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            int x = 0, y = 0, dx = 10, dy = 1;
            foreach (var line in data)
            {
                int amount = int.Parse(line.Substring(1));
                switch (line[0])
                {
                    case 'N':
                        dy += amount;
                        break;
                    case 'E':
                        dx += amount;
                        break;
                    case 'S':
                        dy -= amount;
                        break;
                    case 'W':
                        dx -= amount;
                        break;
                    case 'F':
                        x += amount * dx;
                        y += amount * dy;
                        break;
                    case 'L':
                    {
                        for (int i = 0; i < amount; i += 90)
                        {
                            var o = dx;
                            dx = -dy;
                            dy = o;
                        }

                        break;
                    }
                    case 'R':
                    {
                        for (int i = 0; i < amount; i += 90)
                        {
                            var o = dx;
                            dx = dy;
                            dy = -o;
                        }
                    }
                        break;
                }
            }

            Console.WriteLine($"abs({x}) + abs({y}) = {Math.Abs(x) + Math.Abs(y)}");
        }
    }
}