using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Day1.Problem2();
        }
    }

    public class Data
    {
        public static Task<string[]> GetDataLines(int day, int problem)
        {
            return File.ReadAllLinesAsync(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "data",
                    $"day{day}_problem{problem}.txt"
                )
            );
        }
    }
}
