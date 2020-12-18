using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day18Regex
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            long sum = 0;
            foreach (var line in data)
            {
                long value = EvalNoPrecidence(line);
                sum += value;
                Console.WriteLine($"{line} = {value}");
            }
            
            Console.WriteLine(" ---------------- ");
            Console.WriteLine($"Sum: {sum}");
        }

        private static long EvalNoPrecidence(string equation)
        {
            var m = Regex.Match(equation, @"^(.*)\(([^)]*)\)(.*)$");
            if (m.Success)
            {
                return EvalNoPrecidence($"{m.Groups[1].Value}{EvalNoPrecidence(m.Groups[2].Value)}{m.Groups[3].Value}");
            }

            if (Rx.IsM(equation, @"^(\d+)$", out long value))
                return value;

            Rx.M(equation, @"^(\d+) (.) (\d+)(.*)$", out long a, out char op, out long b, out string rest);

            switch (op)
            {
                case '+':
                    return EvalNoPrecidence($"{a + b}{rest}");
                case '-':
                    return EvalNoPrecidence($"{a - b}{rest}");
                case '*':
                    return EvalNoPrecidence($"{a * b}{rest}");
                case '/':
                    return EvalNoPrecidence($"{a / b}{rest}");
                default:
                    throw new ArgumentException($"Unknown operation '{op}'");
            }
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            long sum = 0;
            foreach (var line in data)
            {
                long value = EvalWithPrecidence(line);
                sum += value;
                Console.WriteLine($"{line} = {value}");
            }
            
            Console.WriteLine(" ---------------- ");
            Console.WriteLine($"Sum: {sum}");
        }

        private static long EvalWithPrecidence(string equation)
        {
            if (Rx.IsM(equation, @"^(\d+)$", out long value))
                return value;
            
            var m = Regex.Match(equation, @"^(.*)\(([^)]*)\)(.*)$");
            if (m.Success)
            {
                return EvalWithPrecidence($"{m.Groups[1].Value}{EvalWithPrecidence(m.Groups[2].Value)}{m.Groups[3].Value}");
            }

            if (Rx.IsM(equation, @"^(.*?)(\d+) \+ (\d+)(.*)$", out string l, out long addLeft, out long addRight, out string r))
            {
                return EvalWithPrecidence($"{l}{addLeft + addRight}{r}");
            }

            Rx.M(equation, @"^(\d+) (.) (\d+)(.*)$", out long a, out char op, out long b, out string rest);

            switch (op)
            {
                case '+':
                    return EvalWithPrecidence($"{a + b}{rest}");
                case '-':
                    return EvalWithPrecidence($"{a - b}{rest}");
                case '*':
                    return EvalWithPrecidence($"{a * b}{rest}");
                case '/':
                    return EvalWithPrecidence($"{a / b}{rest}");
                default:
                    throw new ArgumentException($"Unknown operation '{op}'");
            }
        }
    }
}