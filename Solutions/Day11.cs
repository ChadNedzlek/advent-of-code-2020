using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day11
    {
        public static async Task Problem1()
        {
            char[][] data = (await Data.GetDataLines()).Select(c => c.ToCharArray()).ToArray();

            int CheckSeat(int row, int col)
            {
                if (row < 0 || row >= data.Length)
                    return 0;

                if (col < 0 || col >= data[row].Length)
                    return 0;

                return data[row][col] == '#' ? 1 : 0;
            }

            bool changed;
            do
            {
                changed = false;
                var newData = new char[data.Length][];
                for (int iRow = 0; iRow < data.Length; iRow++)
                {
                    char[] row = data[iRow];
                    newData[iRow] = new char[row.Length];
                    for (int iCol = 0; iCol < row.Length; iCol++)
                    {
                        char seat = row[iCol];

                        int near =
                            CheckSeat(iRow - 1, iCol - 1) +
                            CheckSeat(iRow, iCol - 1) +
                            CheckSeat(iRow + 1, iCol - 1) +
                            CheckSeat(iRow - 1, iCol) +
                            CheckSeat(iRow + 1, iCol) +
                            CheckSeat(iRow - 1, iCol + 1) +
                            CheckSeat(iRow, iCol + 1) +
                            CheckSeat(iRow + 1, iCol + 1);

                        if (near == 0 && seat == 'L')
                        {
                            newData[iRow][iCol] = '#';
                            changed = true;
                        }
                        else if (near >= 4 && seat == '#')
                        {
                            newData[iRow][iCol] = 'L';
                            changed = true;
                        }
                        else
                        {
                            newData[iRow][iCol] = seat;
                        }
                    }

                }

                data = newData;
            } while (changed);

            Console.WriteLine($"Occupied seats: {data.Sum(r => r.Count(c => c == '#'))}");
        }

        public static async Task Problem2()
        {
            char[][] data = (await Data.GetDataLines()).Select(c => c.ToCharArray()).ToArray();

            int CheckSeat(int oRow, int oCol, int dRow, int dCol)
            {
                while (true)
                {
                    int row = oRow + dRow;
                    int col = oCol + dCol;

                    if (row < 0 || row >= data.Length) return 0;

                    if (col < 0 || col >= data[row].Length) return 0;

                    if (data[row][col] == '#')
                    {
                        return 1;
                    }

                    if (data[row][col] == 'L')
                    {
                        return 0;
                    }

                    oRow = row;
                    oCol = col;
                }
            }

            bool changed;
            do
            {
                changed = false;
                var newData = new char[data.Length][];
                for (int iRow = 0; iRow < data.Length; iRow++)
                {
                    char[] row = data[iRow];
                    newData[iRow] = new char[row.Length];
                    for (int iCol = 0; iCol < row.Length; iCol++)
                    {
                        char seat = row[iCol];

                        int near =
                            CheckSeat(iRow, iCol, -1, -1) +
                            CheckSeat(iRow, iCol, 0, -1) +
                            CheckSeat(iRow, iCol, +1, -1) +
                            CheckSeat(iRow, iCol, -1, 0) +
                            CheckSeat(iRow, iCol, +1, 0) +
                            CheckSeat(iRow, iCol, -1, 1) +
                            CheckSeat(iRow, iCol, 0, 1) +
                            CheckSeat(iRow, iCol, +1, 1);

                        if (near == 0 && seat == 'L')
                        {
                            newData[iRow][iCol] = '#';
                            changed = true;
                        }
                        else if (near >= 5 && seat == '#')
                        {
                            newData[iRow][iCol] = 'L';
                            changed = true;
                        }
                        else
                        {
                            newData[iRow][iCol] = seat;
                        }
                    }

                }

                data = newData;
            } while (changed);

            Console.WriteLine($"Occupied seats: {data.Sum(r => r.Count(c => c == '#'))}");
        }
}
}