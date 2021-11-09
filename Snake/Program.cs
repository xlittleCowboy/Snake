using System;
using System.Collections.Generic;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.MainGame();
        }
    }

    class Game
    {
        public static bool play;
        static public void MainGame()
        {
            // Initialization
            Area playArea = new Area();
            Snake snake = new Snake();
            Food food = new Food();
            playArea.Start();
            snake.Start();
            food.Start();
            Console.CursorVisible = false;
            Console.Clear();

            play = true;


            // Play loop
            while (true)
            {
                snake.Move();
                snake.isSnakeDead();
                if (play == false)
                    break;
                food.FoodCheck();
                food.ScoreDraw();
                playArea.DrawArea();
                System.Threading.Thread.Sleep(100 / snake.snakeSpeed);
            }
        }

        static public void End()
        {
            play = false;
            Console.Clear();
            Console.CursorVisible = true;

            if (Food.score >= Food.scoreForWin)
                Console.WriteLine($"You won!\nYour score is: {Food.score}! Congratulations!");
            else
                Console.WriteLine($"You lost!\nYour score is: {Food.score}");
            Console.WriteLine("Press any button to restart!");

            Console.ReadKey();
            Console.Clear();
            Game.MainGame();

        }

        static public void IncorrectValues()
        {
            play = false;
            Console.WriteLine("Incorrect value!");
            Console.ReadKey();
            Console.Clear();
            MainGame();
        }
    }

    class Food
    {
        static public int xFoodPos, yFoodPos;
        static public int score, scoreForWin;
        int rndFoodCord;
        static public char foodSymbol;
        List<int[]> foodCords = new List<int[]>();

        public void Start()
        {
            xFoodPos = yFoodPos = -1;
            score = 0;
            foodSymbol = '$';

            scoreForWin = -Snake.snakeBody.Count + 1;
            for (int i = 1; i < Area.area.GetLength(1) - 1; i++)
            {
                for (int j = 1; j < Area.area.GetLength(0) - 1; j++)
                {
                    if (Area.area[j, i] == ' ')
                        scoreForWin++;
                }
            }
        }

        public void FoodCheck()
        {
            if (xFoodPos == -1)
                FoodSpawn();
            else if (Snake.snakeBody[0][0] == xFoodPos && Snake.snakeBody[0][1] == yFoodPos)
            {
                score++;
                Area.area[xFoodPos, yFoodPos] = Area.area[Snake.snakeBody[0][0], Snake.snakeBody[0][1]];
                Snake.snakeBody.Add(new int[] { xFoodPos, yFoodPos });
                FoodSpawn();
            }
        }

        void FoodSpawn()
        {
            Random rnd = new Random();
            for (int i = 1; i < Area.area.GetLength(1) - 1; i++)
            {
                for (int j = 1; j < Area.area.GetLength(0) - 1; j++)
                {
                    if (Area.area[j, i] == ' ')
                        foodCords.Add(new int[] { j, i });
                }
            }
            if (foodCords.Count != 0)
            {
                rndFoodCord = rnd.Next(0, foodCords.Count);
                xFoodPos = foodCords[rndFoodCord][0];
                yFoodPos = foodCords[rndFoodCord][1];
                Area.area[xFoodPos, yFoodPos] = foodSymbol;
                foodCords.Clear();
            }
        }

        public void ScoreDraw()
        {
            Console.SetCursorPosition(Area.area.GetLength(0) + 1, 0);
            Console.Write($"Score: {score}/{scoreForWin}");
        }
    }

    class Area
    {
        public int width, height;
        static public char[,] area;
        char[,] oldArea;

        public void Start()
        {
            // Window Initialization
            Console.Title = "Snake!";

            //Play Area Initialization
            try
            {
                Console.Write("Play area width: ");
                width = int.Parse(Console.ReadLine()) + 2;
                if (width - 2 <= 0)
                    Game.IncorrectValues();
                Console.Write("Play area height: ");
                height = int.Parse(Console.ReadLine()) + 2;
                if (height - 2 <= 0)
                    Game.IncorrectValues();
                area = new char[width, height];
                oldArea = new char[width, height];
            }
            catch
            {
                Game.IncorrectValues();
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j == 0 || j == width - 1)
                        area[j, i] = '|';
                    else if (i == 0 || i == height - 1)
                        area[j, i] = '-';
                    else
                        area[j, i] = ' ';
                }
            }
        }

        public void DrawArea()
        {

            for (int i = 0; i < area.GetLength(1); i++)
            {
                for (int j = 0; j < area.GetLength(0); j++)
                {
                    if (area[j, i] != oldArea[j, i])
                    {
                        if (area[j, i] == Food.foodSymbol)
                            Console.ForegroundColor = ConsoleColor.Red;
                        else if (area[j, i] == '-' || area[j, i] == '|')
                            Console.ForegroundColor = ConsoleColor.Blue;
                        else if (area[j, i] == Snake.snakeHeadSymbol)
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.SetCursorPosition(j, i);
                        Console.Write(area[j, i]);
                        Console.ResetColor();
                        oldArea[j, i] = area[j, i];
                    }
                }
                Console.WriteLine();
            }
        }

    }

    class Snake
    {
        public int xPos, yPos;
        public int snakeStartLength;
        public int snakeSpeed;
        public char snakeBodySymbol;
        static public char snakeHeadSymbol;
        public char direction = 'R';
        public bool play;
        static public List<int[]> snakeBody;
        ConsoleKeyInfo cki;

        public void Start()
        {
            // Snake Initialization
            try
            {
                Console.Write("Snake speed: ");
                snakeSpeed = int.Parse(Console.ReadLine());
                if (snakeSpeed < 1)
                    Game.IncorrectValues();
            }
            catch
            {
                Game.IncorrectValues();
            }
            xPos = Area.area.GetLength(0) / 2;
            yPos = Area.area.GetLength(1) / 2;
            snakeBodySymbol = '■';
            snakeHeadSymbol = '■';
            snakeStartLength = 3;
            snakeBody = new List<int[]>();
            play = true;

            for (int i = 0; i < snakeStartLength; i++)
            {
                snakeBody.Add(new int[] { xPos - i, yPos });
            }
        }

        public void Move()
        {
            Input();

            Area.area[snakeBody[snakeBody.Count - 1][0], snakeBody[snakeBody.Count - 1][1]] = ' ';
            snakeBody.Insert(0, new int[] { xPos, yPos });
            snakeBody.RemoveAt(snakeBody.Count - 1);
            Area.area[snakeBody[0][0], snakeBody[0][1]] = snakeHeadSymbol;
            for (int i = 1; i < snakeBody.Count; i++)
                Area.area[snakeBody[i][0], snakeBody[i][1]] = snakeBodySymbol;
        }

        void Input()
        {
            if (Console.KeyAvailable)
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.D && direction != 'L')
                    direction = 'R';
                if (cki.Key == ConsoleKey.A && direction != 'R')
                    direction = 'L';
                if (cki.Key == ConsoleKey.W && direction != 'D')
                    direction = 'U';
                if (cki.Key == ConsoleKey.S && direction != 'U')
                    direction = 'D';
            }

            if (direction == 'R')
                xPos += 1;
            if (direction == 'L')
                xPos -= 1;
            if (direction == 'U')
                yPos -= 1;
            if (direction == 'D')
                yPos += 1;
        }

        public void isSnakeDead()
        {
            // Is snake in itself?
            for (int i = 1; i < snakeBody.Count; i++)
                if (snakeBody[0][0] == snakeBody[i][0] && snakeBody[0][1] == snakeBody[i][1] && snakeBody[0][0] != Food.xFoodPos && snakeBody[0][1] != Food.yFoodPos)
                    Game.End();

            // Is snake out of area?
            if (xPos >= Area.area.GetLength(0) - 1 || xPos < 1 || yPos >= Area.area.GetLength(1) - 1 || yPos < 1)
                Game.End();
        }
    }
}
