using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static int screenWidth = 32;
        static int screenHeight = 16;

        static Random random = new Random();

        static int score = 5;

        static bool gameOver = false;

        static Pixel head = new Pixel { xpos = screenWidth / 2, ypos = screenHeight / 2, color = ConsoleColor.Red };

        static string movement = "RIGHT";

        static List<int> bodyX = new List<int>();
        static List<int> bodyY = new List<int>();

        static int berryX = random.Next(1, screenWidth - 2);
        static int berryY = random.Next(1, screenHeight - 2);

        static void Main()
        {
            Console.WindowHeight = screenHeight;
            Console.WindowWidth = screenWidth;

            while (!gameOver)
            {
                Console.Clear();
                DrawBorders();
                HandleCollisions();
                DrawSnake();
                DrawBerry();
                GetInput();
                MoveSnake();
            }

            GameOverScreen();
        }

        static void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < screenWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, screenHeight - 1);
                Console.Write("■");
            }
            for (int i = 0; i < screenHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(screenWidth - 1, i);
                Console.Write("■");
            }
        }

        static void HandleCollisions()
        {
            if (head.xpos == 0 || head.xpos == screenWidth - 1 || head.ypos == 0 || head.ypos == screenHeight - 1)
                gameOver = true;

            for (int i = 0; i < bodyX.Count; i++)
            {
                if (bodyX[i] == head.xpos && bodyY[i] == head.ypos)
                {
                    gameOver = true;
                    return;
                }
            }
            if (head.xpos == berryX && head.ypos == berryY)
            {
                score++;
                berryX = random.Next(1, screenWidth - 2);
                berryY = random.Next(1, screenHeight - 2);
            }
        }

        static void DrawSnake()
        {
            Console.ForegroundColor = head.color;
            Console.SetCursorPosition(head.xpos, head.ypos);
            Console.Write("■");

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < bodyX.Count; i++)
            {
                Console.SetCursorPosition(bodyX[i], bodyY[i]);
                Console.Write("■");
            }
        }

        static void DrawBerry()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(berryX, berryY);
            Console.Write("■");
        }

        static void GetInput()
        {
            DateTime start = DateTime.Now;
            string buttonPressed = "no";
            while (DateTime.Now.Subtract(start).TotalMilliseconds < 500)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow && movement != "DOWN" && buttonPressed == "no")
                    {
                        movement = "UP";
                        buttonPressed = "yes";
                    }
                    else if (key == ConsoleKey.DownArrow && movement != "UP" && buttonPressed == "no")
                    {
                        movement = "DOWN";
                        buttonPressed = "yes";
                    }
                    else if (key == ConsoleKey.LeftArrow && movement != "RIGHT" && buttonPressed == "no")
                    {
                        movement = "LEFT";
                        buttonPressed = "yes";
                    }
                    else if (key == ConsoleKey.RightArrow && movement != "LEFT" && buttonPressed == "no")
                    {
                        movement = "RIGHT";
                        buttonPressed = "yes";
                    }
                }
            }
        }

        static void MoveSnake()
        {
            bodyX.Add(head.xpos);
            bodyY.Add(head.ypos);

            switch (movement)
            {
                case "UP": head.ypos--; break;
                case "DOWN": head.ypos++; break;
                case "LEFT": head.xpos--; break;
                case "RIGHT": head.xpos++; break;
            }

            if (bodyX.Count > score)
            {
                bodyX.RemoveAt(0);
                bodyY.RemoveAt(0);
            }
        }

        static void GameOverScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine($"Game over, Score: {score}");
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    class Pixel
    {
        public int xpos { get; set; }
        public int ypos { get; set; }
        public ConsoleColor color { get; set; }
    }
}
