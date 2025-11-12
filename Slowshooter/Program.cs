using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Slowshooter
{
    internal class Program
    {

        static string playField =
@"+-------+     +-------+
|       |     |       |
|       |     |       |
|       |     |       |
|       |     |       |
|       |     |       |
+-------+     +-------+";

        static bool isPlaying = true;

        static bool p1_placingMine;
        static bool p2_placingMine;

        //player lives
        static int p1_lives = 3;
        static int p2_lives = 3;

        // player input 
        static int p1_x_input;
        static int p1_y_input;
        

        static int p2_x_input;
        static int p2_y_input;

        // player 1 pos
        static int p1_x_pos = 4;
        static int p1_y_pos = 3;

        // player 2 pos
        static int p2_x_pos = 18;
        static int p2_y_pos = 3;

        // bounds for player movement
        static (int, int) p1_min_max_x = (1, 7);
        static (int, int) p1_min_max_y = (1, 5);
        static (int, int) p2_min_max_x = (15, 21);
        static (int, int) p2_min_max_y = (1, 5);

        //mines Lists
        static List<(int, int)> p1_minesPositions = new List<(int, int)>();
        static List<(int, int)> p2_minesPositions = new List<(int, int)>();

        // what turn is it? will be 0 after game is drawn the first time
        static int turn = -1;

        // contains the keys that player 1 and player 2 are allowed to press
        static (char[], char[]) allKeybindings = (new char[]{ 'W', 'A', 'S', 'D', 'E' }, new char[]{ 'J', 'I', 'L', 'K', 'O' });
        static ConsoleColor[] playerColors = { ConsoleColor.Red, ConsoleColor.Blue };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            
            while(isPlaying)
            {
                ProcessInput();
                Update();
                Draw();
            }

            Console.Clear();
        }

        static void ProcessInput()
        {
            // if this isn't here, input will block the game before drawing for the first time
            if (turn == -1) return;

            // reset input
            p1_x_input = 0;
            p1_y_input = 0;
            p2_x_input = 0;
            p2_y_input = 0;
            p1_placingMine = false;
            p2_placingMine = false;



            char[] allowedKeysThisTurn; // different keys allowed on p1 vs. p2 turn

            // choose which keybindings to use
            if (turn % 2 == 0) allowedKeysThisTurn = allKeybindings.Item1;
            else allowedKeysThisTurn = allKeybindings.Item2;

            // get the current player's input
            ConsoleKey input = ConsoleKey.NoName;
            while (!allowedKeysThisTurn.Contains(((char)input)))
            {
                input = Console.ReadKey(true).Key;
            }

            // check all input keys 
            if (input == ConsoleKey.A) p1_x_input = -1;
            if (input == ConsoleKey.D) p1_x_input = 1;
            if (input == ConsoleKey.W) p1_y_input = -1;
            if (input == ConsoleKey.S) p1_y_input = 1;
            if (input == ConsoleKey.E)
            {
                p1_placingMine = true;
            }


            if (input == ConsoleKey.J) p2_x_input = -1;
            if (input == ConsoleKey.L) p2_x_input = 1;
            if (input == ConsoleKey.I) p2_y_input = -1;
            if (input == ConsoleKey.K) p2_y_input = 1;
            if (input == ConsoleKey.O)
            {
                p2_placingMine = true;
            }
        }

        static void Update()
        {
            // update players' positions based on input
            p1_x_pos += p1_x_input;
            p1_x_pos = p1_x_pos.Clamp(p1_min_max_x.Item1, p1_min_max_x.Item2);

            p1_y_pos += p1_y_input;
            p1_y_pos = p1_y_pos.Clamp(p1_min_max_y.Item1, p1_min_max_y.Item2);

            p2_x_pos += p2_x_input;
            p2_x_pos = p2_x_pos.Clamp(p2_min_max_x.Item1, p2_min_max_x.Item2);

            p2_y_pos += p2_y_input;
            p2_y_pos = p2_y_pos.Clamp(p2_min_max_y.Item1, p2_min_max_y.Item2);

            if (p1_placingMine)
            {
                p2_minesPositions.Add((p1_x_pos + 14, p1_y_pos));
            }

            if (p2_placingMine)
            {
                p1_minesPositions.Add((p2_x_pos - 14, p2_y_pos));
            }

            for (int i = 0; i < p1_minesPositions.Count; i++)
            {
                if (p1_minesPositions[i].Equals((p1_x_pos, p1_y_pos)))
                {
                    p1_lives--;
                    p1_minesPositions.RemoveAt(i);
                }
            }

            for (int i = 0; i < p2_minesPositions.Count; i++)
            {
                if (p2_minesPositions[i].Equals((p2_x_pos, p2_y_pos)))
                {
                    p2_lives--;
                    p2_minesPositions.RemoveAt(i);
                }
            }

            if (p1_lives <= 0)
            {
                isPlaying = false;

                Console.Clear();
                Console.WriteLine("Player 2 wins!");
                Console.ReadKey(true);
            }

            if (p2_lives <= 0)
            {
                isPlaying = false;

                Console.Clear();
                Console.WriteLine("Player 1 wins!");
                Console.ReadKey(true);
            }

            turn += 1;
        }

        static void Draw()
        {
            // draw the background (playfield)
            Console.SetCursorPosition(0, 0);
            Console.Write(playField);

                // draw player 1
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = playerColors[0];
            Console.SetCursorPosition(p1_x_pos, p1_y_pos);
            Console.Write("O");


                // draw player 2
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = playerColors[1];
            Console.SetCursorPosition(p2_x_pos, p2_y_pos);
            Console.Write("O");

            Console.SetCursorPosition(0, 7);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Lives: {p1_lives}           Lives: {p2_lives}");

            // draw the Turn Indicator
            Console.SetCursorPosition(3, 8);
            Console.ForegroundColor = playerColors[turn % 2];

            Console.Write($"PLAYER {turn % 2 + 1}'S TURN!");


            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nUSE WASD or IJKL to move");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
