using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    public class Day22
    {
        public static async Task Problem1()
        {
            var data = await Data.GetDataLines();
            List<int> player1 = data[1..((data.Length - 1) / 2)].Select(int.Parse).ToList();
            List<int> player2 = data[((data.Length - 1) / 2 + 2)..].Select(int.Parse).ToList();
            while (player1.Count > 0 && player2.Count > 0)
            {
                Console.WriteLine("---- Round ----");
                Console.WriteLine($"Player 1's deck: {string.Join(", ", player1)}");
                Console.WriteLine($"Player 2's deck: {string.Join(", ", player2)}");
                int c1 = player1[0], c2 = player2[0];
                player1.RemoveAt(0);
                player2.RemoveAt(0);
                Console.WriteLine($"Player 1 plays: {c1}");
                Console.WriteLine($"Player 2 plays: {c2}");
                if (c1 > c2)
                {
                    Console.WriteLine("Player 1 windows the round!");
                    player1.Add(c1);
                    player1.Add(c2);
                }
                else
                {
                    Console.WriteLine("Player 2 windows the round!");
                    player2.Add(c2);
                    player2.Add(c1);
                }
            }
            
            Console.WriteLine("---- End ----");
            Console.WriteLine($"Player 1's deck: {string.Join(", ", player1)}");
            Console.WriteLine($"Player 2's deck: {string.Join(", ", player2)}");

            var winner = player1.Count == 0 ? player2 : player1;
            Console.WriteLine($"Score : {winner.Select((c, i) => c * (winner.Count - i)).Sum()}");
        }

        public static async Task Problem2()
        {
            var data = await Data.GetDataLines();
            List<int> player1 = data[1..((data.Length - 1) / 2)].Select(int.Parse).ToList();
            List<int> player2 = data[((data.Length - 1) / 2 + 2)..].Select(int.Parse).ToList();

            int game = 0;
            int PlayGame(List<int> p1, List<int> p2)
            {
                game++;
                int myGame = game;
                //Console.WriteLine($"=== Game {myGame} ===");
                HashSet<string> signatures = new HashSet<string>();

                string StateSig() => $"Player 1:{string.Join(",", p1)} - Player 2:{string.Join(",", p2)}";

                int round = 0;
                int PlayRound()
                {
                    round++;
                    //Console.WriteLine();
                    //Console.WriteLine($"-- Round {round} (Game {myGame})");
                    //Console.WriteLine($"Player 1's deck: {string.Join(", ", p1)}");
                    //Console.WriteLine($"Player 2's deck: {string.Join(", ", p2)}");
                    if (p1.Count == 0)
                        return -2;
                    if (p2.Count == 0)
                        return -1;
                    string state = StateSig();
                    if (!signatures.Add(state))
                    {
                        return -1;
                    }

                    if (myGame == 1)
                    {
                        // Console.WriteLine($"Seen {signatures.Count} unique games...");
                    }

                    int c1 = p1[0], c2 = p2[0];
                    p1.RemoveAt(0);
                    p2.RemoveAt(0);
                    //Console.WriteLine($"Player 1 plays: {c1}");
                    //Console.WriteLine($"Player 2 plays: {c2}");

                    int winner = 0;
                    if (c1 <= p1.Count && c2 <= p2.Count)
                    {
                        int RecursiveCombat()
                        {
                            return PlayGame(p1.Take(c1).ToList(), p2.Take(c2).ToList());
                        }
                        //Console.WriteLine("Playing a sub-game to determine the winner...");
                        //Console.WriteLine();
                        winner = RecursiveCombat();
                        //Console.WriteLine($"... anyway, back to game {myGame}");
                    }
                    else
                    {
                        winner = c1 > c2 ? 1 : 2;
                    }

                    if (winner == 1)
                    {
                        //Console.WriteLine("Player 1 windows the round!");
                        p1.Add(c1);
                        p1.Add(c2);
                        return 1;
                    }
                    
                    //Console.WriteLine("Player 2 windows the round!");
                    p2.Add(c2);
                    p2.Add(c1);
                    return 2;
                }

                int playRound;
                while ((playRound = PlayRound()) > 0)
                {
                    // do nothing
                }

                // Console.WriteLine($"The winner of game {myGame} is player {-playRound}!");

                return -playRound;
            }

            int winner = PlayGame(player1, player2);

            var winningDeck = winner == 1 ? player1 : player2;
            Console.WriteLine($"Score : {winningDeck.Select((c, i) => c * (winningDeck.Count - i)).Sum()}");
        }
    }
}