using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

namespace Snake
{
    class Program
    {
        static int screenWidth = 32;
        static int screenHeight = 16;

        static Random random = new Random();

        static int score = 1;
        static bool gameOver = false;

        static Snake snake = new Snake(score) { position = new Point(screenWidth / 2, screenHeight / 2) };
        static Berry berry;
        static ConsoleRenderer renderer;

        static void Main()
        {

            Console.SetWindowSize(screenWidth + 1, screenHeight + 1);
            Console.SetBufferSize(screenWidth + 1, screenHeight + 1);

            WindowHeight = screenHeight;
            WindowWidth = screenWidth;

            renderer = new ConsoleRenderer(screenWidth, screenHeight);
            snake = new Snake(score);
            berry = new Berry(snake);

            while (!gameOver)
            {
                Clear();
                renderer.DrawBorders();
                HandleCollisions();
                snake.GetInput();
                snake.Move();
                renderer.Render(snake);
                renderer.Render(berry);
                Thread.Sleep(100);
            }

            GameOverScreen();
        }

        static void HandleCollisions()
        {

            if (snake.position.X == 0 || snake.position.X == screenWidth - 1 || snake.position.Y == 0 || snake.position.Y == screenHeight - 1)
                gameOver = true;

            for (int i = 0; i < snake.bodyX.Count; i++)
            {
                if (snake.bodyX[i] == snake.position.X && snake.bodyY[i] == snake.position.Y)
                {
                    gameOver = true;
                    return;
                }
            }

            if (snake.position.X == berry.position.X && snake.position.Y == berry.position.Y)
            {
                score++;
                berry.Respawn();

                snake.bodyX.Add(snake.bodyX[snake.bodyX.Count - 1]);
                snake.bodyY.Add(snake.bodyY[snake.bodyY.Count - 1]);
            }
        }

        static void GameOverScreen()
        {
            Clear();
            SetCursorPosition(screenWidth / 5, screenHeight / 2);
            WriteLine($"Game over, Score: {score}");
            SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            WriteLine("Press any key to exit...");
            ReadKey();
        }
    }

    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    class ConsoleRenderer
    {
        private int screenWidth;
        private int screenHeight;

        public ConsoleRenderer(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
        }

        public void DrawBorders()
        {
            ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < screenWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");
                SetCursorPosition(i, screenHeight - 1);
                Write("■");
            }
            for (int i = 0; i < screenHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");
                SetCursorPosition(screenWidth - 1, i);
                Write("■");
            }
        }

        public void Render(Snake snake)
        {
            if (snake.position.X >= 0 && snake.position.X < screenWidth && snake.position.Y >= 0 && snake.position.Y < screenHeight)
            {
                ForegroundColor = snake.color;
                SetCursorPosition(snake.position.X, snake.position.Y);
                Write("■");
            }

            ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < snake.bodyX.Count; i++)
            {
                if (snake.bodyX[i] >= 0 && snake.bodyX[i] < screenWidth && snake.bodyY[i] >= 0 && snake.bodyY[i] < screenHeight)
                {
                    SetCursorPosition(snake.bodyX[i], snake.bodyY[i]);
                    Write("■");
                }
            }
        }


        public void Render(Berry berry)
        {
            ForegroundColor = berry.color;
            SetCursorPosition(berry.position.X, berry.position.Y);
            Write("■");
        }
    }

    class Snake : GameObject
    {
        static int screenWidth = 32;
        static int screenHeight = 16;
        static Random random = new Random();

        private Direction movement = Direction.RIGHT;

        public List<int> bodyX = new List<int>();
        public List<int> bodyY = new List<int>();
        private int score;

        public Snake(int initialScore)
        {
            score = initialScore;
            color = ConsoleColor.Red;

            position = new Point(screenWidth / 2, screenHeight / 2);
        }

        public override void Draw()
        {
            ForegroundColor = color;
            SetCursorPosition(position.X, position.Y);
            Write("■");

            ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < bodyX.Count; i++)
            {
                SetCursorPosition(bodyX[i], bodyY[i]);
                Write("■");
            }
        }

        public void Move()
        {
            bodyX.Add(position.X);
            bodyY.Add(position.Y);

            switch (movement)
            {
                case Direction.UP: position.Y--; break;
                case Direction.DOWN: position.Y++; break;
                case Direction.LEFT: position.X--; break;
                case Direction.RIGHT: position.X++; break;
            }

            if (bodyX.Count > score)
            {
                bodyX.RemoveAt(0);
                bodyY.RemoveAt(0);
            }
        }

        public void GetInput()
        {
            string buttonPressed = "no";
            if (KeyAvailable)
            {
                ConsoleKey key = ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && movement != Direction.DOWN && buttonPressed == "no")
                {
                    movement = Direction.UP;
                    buttonPressed = "yes";
                }
                else if (key == ConsoleKey.DownArrow && movement != Direction.UP && buttonPressed == "no")
                {
                    movement = Direction.DOWN;
                    buttonPressed = "yes";
                }
                else if (key == ConsoleKey.LeftArrow && movement != Direction.RIGHT && buttonPressed == "no")
                {
                    movement = Direction.LEFT;
                    buttonPressed = "yes";
                }
                else if (key == ConsoleKey.RightArrow && movement != Direction.LEFT && buttonPressed == "no")
                {
                    movement = Direction.RIGHT;
                    buttonPressed = "yes";
                }
            }
        }
    }

    class Berry : GameObject
    {
        static Random random = new Random();

        private Snake snake;

        public Berry(Snake snake)
        {
            this.snake = snake;
            color = ConsoleColor.Cyan;
            Respawn();
        }

        public override void Draw()
        {
            ForegroundColor = color;
            SetCursorPosition(position.X, position.Y);
            Write("■");
        }

        public void Respawn()
        {
            do
            {
                position = new Point(random.Next(1, 32 - 2), random.Next(1, 16 - 2));
            } while (IsBerryOnSnake(position.X, position.Y));
        }

        private bool IsBerryOnSnake(int berryX, int berryY)
        {
            for (int i = 0; i < snake.bodyX.Count; i++)
            {
                if (snake.bodyX[i] == berryX && snake.bodyY[i] == berryY)
                {
                    return true;
                }
            }
            return false;
        }
    }

    abstract class GameObject
    {
        public Point position;

        public ConsoleColor color { get; set; }

        public abstract void Draw();
    }
}