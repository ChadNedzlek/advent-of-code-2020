using System;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public static class Deconstructors
    {
        public static void Deconstruct(this Match match, out string a, out string b, out string c, out string d)
        {
            if (!match.Success)
            {
                throw new ArgumentException("Match is not successful");
            }

            if (match.Groups.Count != 5)
            {
                throw new ArgumentException("Match does not have 4 groups");
            }

            a = match.Groups[1].Value;
            b = match.Groups[2].Value;
            c = match.Groups[3].Value;
            d = match.Groups[4].Value;
        }
    }
}