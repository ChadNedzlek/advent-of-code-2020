using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day21
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            List<Food> foods = new List<Food>();
            foreach (var line in data)
            {
                var m = Regex.Match(line, @"^(?:([a-z]*) )*\(contains (?:([a-z]*)(?:, |\)))*$");
                var ingredients = m.Groups[1].Captures.Select(c => c.Value).ToHashSet();
                var allergens = m.Groups[2].Captures.Select(c => c.Value).ToHashSet();
                foods.Add(new Food(ingredients, allergens));
            }
            Dictionary<string, HashSet<string>> allergenIngredients = new Dictionary<string, HashSet<string>>();
            foreach (var food in foods)
            {
                foreach (var a in food.Allergens)
                {
                    if (allergenIngredients.TryGetValue(a, out var iList))
                    {
                        iList.IntersectWith(food.Ingredients);
                    }
                    else
                    {
                        allergenIngredients.Add(a, food.Ingredients.ToHashSet());
                    }
                }
            }

            while (allergenIngredients.Values.Any(i => i.Count > 1))
            {
                foreach (var (a, i) in allergenIngredients)
                {
                    if (i.Count == 1)
                    {
                        foreach (var (aRem, iRem) in allergenIngredients)
                        {
                            if (i != iRem && iRem.Remove(i.First()))
                            {
                                iRem.ExceptWith(i);
                                break;
                            }
                        }
                    }
                }
            }

            var haveAllergen = allergenIngredients.Values.SelectMany(i => i).ToHashSet();
            var noAllergenInstances = foods.Sum(f => f.Ingredients.Count(i => !haveAllergen.Contains(i)));

            Console.WriteLine($"Ingredient instances with no allergens : {noAllergenInstances}");

            Console.WriteLine($"Alergen list: {string.Join(",", allergenIngredients.OrderBy(a => a.Key).Select(a => a.Value.First()))}");
        }

        public class Food
        {
            public Food(HashSet<string> ingredients, HashSet<string> allergens)
            {
                Ingredients = ingredients;
                Allergens = allergens;
            }

            public HashSet<string> Ingredients { get; set; }
            public HashSet<string> Allergens { get; set; }
        }
    }
}