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
            CheeseGame.PlayGame();
            
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Represents a single cell of the grid
    /// </summary>
    public class Point
    {
        //status of each cell in the grid
        public enum PointStatus
        {
            Empty, Cheese, Mouse, Cat, CatAndCheese
        }

        //X coordinate value
        public int X { get; set; }
        //Y coordinate value
        public int Y { get; set; }
        //what does the cell contain, or is it empty
        public PointStatus Status { get; set; }

        /// <summary>
        /// CONSTRUCTOR, set the coordinates of the cell, and set status to Empty
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public Point(int x, int y) {
            this.X = x;
            this.Y = y;
            //set all declarations of Point to "Empty" by default
            this.Status = PointStatus.Empty;
        }
    }

    /// <summary>
    /// Hero of game.  Class responsible for holding status of the Mouse
    /// </summary>
    public class Mouse
    {
        //where is Mouse currently
        public Point Position { get; set; }
        //how much energy the Mouse has left
        public int Energy { get; set; }
        //whether or not a Cat has gotten the Mouse
        public bool HasBeenPouncedOn { get; set; }

        /// <summary>
        /// CONSTRUCTOR, initialize the Mouse and set its energy to 50
        /// </summary>
        public Mouse()
        {
            this.Energy = 50;
            this.HasBeenPouncedOn = false;
        }
    }

    /// <summary>
    /// Represents a Cat in the game, our villian.
    /// </summary>
    public class Cat
    {
        //where is the Cat
        public Point Position { get; set; }

        /// <summary>
        /// CONSTRUCTOR, initializes the Cat
        /// </summary>
        public Cat() { }
    }

    /// <summary>
    /// Core of the game.  The Grid owns the Points, the Mouse, and the Cheese
    /// </summary>
    public class CheeseNibbler
    {
        //multidimensional Array of Points, the grid itself
        public Point[,] Grid { get; set; }
        //represents the Mouse object
        public Mouse Mouse { get; set; }
        //represents the Cheese (REFERENCES a Point on the Grid)
        public Point Cheese { get; set; }
        //holds how many cheeses the mouse has consumed
        public int CheeseCount { get; set; }
        //keeps track of game Rounds
        public int Round { get; set; }
        //Holds all Cats
        public List<Cat> CatList { get; set; }

        Random RNG = new Random();

        /// <summary>
        /// CONSTRUCTOR, initializes Grid, Mouse, Cat List, Mouse position, and places the Cheese
        /// </summary>
        public CheeseNibbler()
        {
            //initializing the Grid array
            this.Grid = new Point[10, 10];
            //fill each cell of the Grid with a Point
            for (int y = 0; y < this.Grid.GetLength(1); y++)
            {
                for (int x = 0; x < this.Grid.GetLength(0); x++)
                {
                    //
                    this.Grid[x, y] = new Point(x, y);
                }
            }

            //initialize the Mouse and randomly place its Position (Point) on the Grid
            Mouse = new Mouse();
            this.Mouse.Position = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
  //this.Mouse.Status = Point.PointStatus.Mouse;

            //place the Cheese on the Grid
            PlaceCheese();

            //initialize the Cats List
            CatList = new List<Cat>();
        }

        /// <summary>
        /// Displays the grid to the user
        /// </summary>
        public void DrawGrid()
        {
            Console.Clear();
            //loop through every Point in the Grid
            for (int y = 0; y < this.Grid.GetLength(1); y++)
            {
                for (int x = 0; x < this.Grid.GetLength(0); x++)
                {
                    //check the status of each Point
                    //print a specific image for each PointStatus type
                    switch (this.Grid[x, y].Status)
                    {
                        //found a Cat space
                        case Point.PointStatus.Cat:
                            Console.Write("[X]");
                            break;
                        //found a Cat/Cheese space (they can occupy the same cell)
                        case Point.PointStatus.CatAndCheese:
                            Console.Write("[X]");
                            break;
                        //found a Cheese space
                        case Point.PointStatus.Cheese:
                            Console.Write("[C]");
                            break;
                        //found a Mouse space
                        case Point.PointStatus.Mouse:
                            Console.Write("[M]");
                            break;
                        //for anything else (should be an Empty space)
                        default:
                            Console.Write("[ ]");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Handles the logic for determining a valid user INPUT
        /// </summary>
        /// <returns>valid user key press</returns>
        public ConsoleKey GetUserMove()
        {
            //instructions on what keys to press
            Console.WriteLine("Press an arrow key to move Mouse");
            //keeps while loop going until user input is valid
            bool validInput = false;
            //continue to check user input until a valid input is received
            while (!validInput)
            {
                //put "true" into Console.ReadKey so that input isn't printed to screen
                ConsoleKeyInfo input = Console.ReadKey(true);
                //check to see if user input is an arrow key only
                if (input.Key == ConsoleKey.LeftArrow || input.Key == ConsoleKey.RightArrow || input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow)
                {
                    //if using an arrow key, but it tries moving you past the boundaries of the grid then the move is invalid, otherwise it will pass and be used to move the Mouse
                    if (ValidMove(input.Key))
                    {
                        validInput = true;
                        return input.Key;
                    }
                    else Console.WriteLine("Move not valid");
                }
                //if not an arrow...
                else
                {
                    Console.WriteLine("Invalid Key");
                }
            }
            //this is used a "place holder" and will never be used (compiler requires a return here)
            //while loop will run until a valid user input and valid user move is reached
            return ConsoleKey.UpArrow;
        }

        /// <summary>
        /// Handles the logic for determining a valid user MOVE
        /// </summary>
        /// <param name="input">user move up, down, left, or right</param>
        /// <returns>true if mouse stays on grid</returns>
        public bool ValidMove(ConsoleKey input)
        {
            //get current Mouse X and Y position
            int currentMouseX = this.Mouse.Position.X;
            int currentMouseY = this.Mouse.Position.Y;
            switch (input)
            {
                //subtract or add to x or y first before checking conditional statement
                //see if x or y value after moving 1 space remains on grid
                //return true if on Grid
                case ConsoleKey.LeftArrow:
                    return --currentMouseX >= 0;
                case ConsoleKey.RightArrow:
                    return ++currentMouseX <= this.Grid.GetLength(0);
                case ConsoleKey.UpArrow:
                    return --currentMouseY >= 0;
                case ConsoleKey.DownArrow:
                    return ++currentMouseY <= this.Grid.GetLength(1);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the logic for moving the Mouse
        /// </summary>
        /// <param name="input">validated user input</param>
        /// <returns>True if Cheese was found</returns>
        public bool MoveMouse(ConsoleKey input)
        {
            //get the current X and Y position of the Mouse
            int newMouseX = this.Mouse.Position.X;
            int newMouseY = this.Mouse.Position.Y;
            //change the values of the Mouse position based on user's input "move"
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    newMouseX--;
                    break;
                case ConsoleKey.RightArrow:
                    newMouseX++;
                    break;
                case ConsoleKey.UpArrow:
                    newMouseY--;
                    break;
                case ConsoleKey.DownArrow:
                    newMouseY++;
                    break;
                default:
                    return false;
            }
            //get the point from the grid for the new position
            Point mouseAfterMove = this.Grid[newMouseX, newMouseY];
            //check if the new position is a Cheese
            if (mouseAfterMove.Status == Point.PointStatus.Cheese)
            {
                //FOUND THE CHEESE
                //increase CheeseCount
                CheeseCount++;
                //set a new Cheese to find
                PlaceCheese();
                if (CheeseCount % 2 == 0)
                {
                    AddCat();
                }
    //***NOT SURE IF THIS CODE WILL STILL BE NECESSARY***
                //clearing the old status from Mouse to Empty
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                //move Mouse to new position
                this.Mouse.Position = mouseAfterMove;
                //change the new position from status Cheese to Mouse
                this.Mouse.Position.Status = Point.PointStatus.Mouse;
                //increase Mouse energy by 10 for finding Cheese
                Mouse.Energy += 10;
                return true;
            }
            else
            {
                //DID NOT FIND THE CHEESE (found an Empty space)
                //**same logic as above except for Empty status, not Cheese**
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                this.Mouse.Position = mouseAfterMove;
                this.Mouse.Position.Status = Point.PointStatus.Mouse;
                //decrease Mouse's energy by 1 for every move
                Mouse.Energy--;
                return false;
            }
        }

        /// <summary>
        /// Handles the logic for placing the Cheese reference.  Ensures that the cheese only appears on an empty Point
        /// </summary>
        public void PlaceCheese()
        {
            //choose a random Point to place the Cheese on the Grid
            this.Cheese = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
            //make sure the cell is Empty, if not choose another Point
            while (this.Cheese.Status != Point.PointStatus.Empty)
            {
                this.Cheese = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
            }
            //if the cell is Empty then change its status to Cheese
            this.Cheese.Status = Point.PointStatus.Cheese;
        }

        /// <summary>
        /// Handles the logic for creating a new Cat
        /// </summary>
        public void AddCat()
        {
            //Create a new Cat object, place it on Grid, and add it to the List of Cats
            Cat newCat = new Cat();
            PlaceCat(newCat);
            this.CatList.Add(newCat);
        }

        /// <summary>
        /// Handles logic for placing a Cat
        /// </summary>
        /// <param name="cat">Cat object</param>
        public void PlaceCat(Cat cat)
        {
            //choose a random Point to place the newCat on the Grid
            cat.Position = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
            //make sure the cell is Empty, if not choose another Point
            while (cat.Position.Status != Point.PointStatus.Empty)
            {
                cat.Position = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
            }
            //if the cell is Empty then change its status to Cat
            cat.Position.Status = Point.PointStatus.Cat;
        }



        public void PlayGame()
        {
            //cheeseFound is end condition
            bool cheeseFound = false;
            //play the game while the Cheese has not been found
            while (!cheeseFound)
            {
                //draw the Grid
                this.DrawGrid();
                //get valid user input, if so move the Mouse
                //check if MoveMouse is true (if so, Cheese was "found")
                if (this.MoveMouse(this.GetUserMove()))
                {
                    //if cheese found, redraw Grid one last time then exit game loop
                    this.DrawGrid();
                    cheeseFound = true;
                }
                //increase round counter for each play of game when Cheese not found
                this.Round++;
            }
            Console.WriteLine("You Won");
            Console.WriteLine("It took you {0} rounds", this.Round);
        }
    }
}
