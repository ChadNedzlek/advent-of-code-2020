using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day5
    {
        private static int SeatId(int row, int seat) => row * 8 + seat;

        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            int seatId = 0;
            foreach (var line in data)
            {
                var row = Convert.ToInt32(line.Substring(0, 7).Replace('F','0').Replace('B','1'), 2);
                var seat = Convert.ToInt32(line.Substring(7, 3).Replace('L','0').Replace('R','1'), 2);

                seatId = Math.Max(seatId, SeatId(row, seat));
            }

            Console.WriteLine($"Silly format highest seat: {seatId}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            var set = new HashSet<int>(Enumerable.Range(0, 128*8));
            foreach (var line in data)
            {
                var row = Convert.ToInt32(line.Substring(0, 7).Replace('F','0').Replace('B','1'), 2);
                var seat = Convert.ToInt32(line.Substring(7, 3).Replace('L','0').Replace('R','1'), 2);
                set.Remove(SeatId(row, seat));
            }
            
            for (int i = 0; set.Contains(i); i++)
            {
                set.Remove(i);
            }

            for (int i = 933; set.Contains(i); i--)
            {
                set.Remove(i);
            }

            Console.WriteLine($"Silly format missing seat: {set.First()}");
        }
    }
}