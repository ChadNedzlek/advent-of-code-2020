using System;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day25
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            int cardPublic = int.Parse(data[0]);
            int doorPublic = int.Parse(data[1]);

            long Transform(int s, int l)
            {
                long value = 1;
                for (int i = 0; i < l; i++)
                {
                    value *= s;
                    value %= 20201227;
                }

                return value;
            }

            int FindLoop(int key)
            {
                long value = 1;
                for (int i = 0;; i++)
                {
                    value *= 7;
                    value %= 20201227;
                    if (value == key)
                    {
                        return i + 1;
                    }
                }
            }
            
            int cardLoop = FindLoop(cardPublic);
            Console.WriteLine($"Card loop: {cardLoop}");
            int doorLoop = FindLoop(doorPublic);
            Console.WriteLine($"Door loop: {doorLoop}");

            var enc = Transform(cardPublic, doorLoop);
            Console.WriteLine($"Enc key: {enc}");
        }
    }
}