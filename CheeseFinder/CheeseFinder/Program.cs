using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            CheeseNibbler CheeseGame = new CheeseNibbler();

            bool cheeseFound = false;
            while (!cheeseFound)
            {
                CheeseGame.DrawGrid();
                if (CheeseGame.MoveMouse(CheeseGame.GetUserMove()))
                {
                    CheeseGame.DrawGrid();
                    cheeseFound = true;
                }
                CheeseGame.Round++;
            }
            Console.WriteLine("You Won");
            Console.WriteLine("It took you {0} rounds", CheeseGame.Round);

            Console.ReadKey();
        }
    }

    public class Point
    {
        public enum PointStatus
        {
            Empty, Cheese, Mouse
        }
        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }

        public Point(int x, int y) {
            this.X = x;
            this.Y = y;
            this.Status = PointStatus.Empty;
        }
    }

    public class CheeseNibbler
    {
        public Point[,] Grid { get; set; }
        public Point Mouse { get; set; }
        public Point Cheese { get; set; }
        public int Round { get; set; }
        Random RNG = new Random();

        public CheeseNibbler()
        {
            this.Grid = new Point[10, 10];
            for (int y = 0; y < this.Grid.GetLength(1); y++)
            {
                for (int x = 0; x < this.Grid.GetLength(0); x++)
                {
                    this.Grid[x, y] = new Point(x, y);
                }
            }
            this.Mouse = Grid[this.RNG.Next(10), this.RNG.Next(10)];
            this.Mouse.Status = Point.PointStatus.Mouse;

            this.Cheese = Grid[this.RNG.Next(10), this.RNG.Next(10)];
            this.Cheese.Status = Point.PointStatus.Cheese;
            while (this.Cheese == this.Mouse)
            {
                this.Cheese = new Point(this.RNG.Next(11), this.RNG.Next(11));
            }
        }

        public void DrawGrid()
        {
            Console.Clear();

            for (int y = 0; y < this.Grid.GetLength(1); y++)
            {
                for (int x = 0; x < this.Grid.GetLength(0); x++)
                {
                    switch (Grid[x, y].Status)
                    {
                        case Point.PointStatus.Empty:
                            Console.Write("[ ]");
                            break;
                        case Point.PointStatus.Cheese:
                            Console.Write("[C]");
                            break;
                        case Point.PointStatus.Mouse:
                            Console.Write("[M]");
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        public ConsoleKey GetUserMove()
        {
            Console.WriteLine("Press an arrow key to move Mouse");
            bool validInput = false;
            while (!validInput)
            {
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == ConsoleKey.LeftArrow || input.Key == ConsoleKey.RightArrow || input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow)
                {
                    if (ValidMove(input.Key))
                    {
                        validInput = true;
                        return input.Key;
                    }
                    else Console.WriteLine("Move not valid");
                }
                else
                {
                    Console.WriteLine("Invalid Key");
                }
            }
            return ConsoleKey.UpArrow;
        }

        public bool ValidMove(ConsoleKey input)
        {
            int x = this.Mouse.X;
            int y = this.Mouse.Y;
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    return --x >= 0;
                case ConsoleKey.RightArrow:
                    return ++x <= 9;
                case ConsoleKey.UpArrow:
                    return --y >= 0;
                case ConsoleKey.DownArrow:
                    return ++y <= 9;
                default:
                    return false;
            }
        }

        public bool MoveMouse(ConsoleKey input)
        {
            int x = this.Mouse.X;
            int y = this.Mouse.Y;
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    x--;
                    break;
                case ConsoleKey.RightArrow:
                    x++;
                    break;
                case ConsoleKey.UpArrow:
                    y--;
                    break;
                case ConsoleKey.DownArrow:
                    y++;
                    break;
                default:
                    return false;
            }
            Point mouseAfterMove = this.Grid[x, y];

            if (mouseAfterMove.Status == Point.PointStatus.Cheese)
            {
                this.Mouse.Status = Point.PointStatus.Empty;
                this.Mouse = mouseAfterMove;
                this.Mouse.Status = Point.PointStatus.Mouse;
                return true;
            }
            else
            {
                this.Mouse.Status = Point.PointStatus.Empty;
                this.Mouse = mouseAfterMove;
                this.Mouse.Status = Point.PointStatus.Mouse;
                return false;
            }

        }
    }
}
