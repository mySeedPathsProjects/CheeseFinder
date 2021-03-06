﻿using System;
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
            Console.WindowWidth = 120;
            CheeseNibbler CheeseGame = new CheeseNibbler();
            CheeseGame.PlayGame();
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
            this.Mouse.Position.Status = Point.PointStatus.Mouse;

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
                        //found a Cat/Cheese space (they can occupy the same cell)
                        case Point.PointStatus.CatAndCheese:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[  =^..^=  ]");
                            break;
                        //found a Cheese space
                        case Point.PointStatus.Cheese:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("[   C H Z  ]");
                            break;
                        //found a Mouse space
                        case Point.PointStatus.Mouse:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("[ ~~(__^·> ]");
                            break;
                        //for anything else (should be an Empty space)
                        default:
                            Console.Write("[          ]");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }
            Console.WriteLine("Your remaining energy: {0}", this.Mouse.Energy);
            Console.WriteLine("Amount of Cheese found: {0}", this.CheeseCount);
            Console.WriteLine();
        }

        /// <summary>
        /// Handles the logic for determining a valid user INPUT
        /// </summary>
        /// <returns>valid user key press</returns>
        public ConsoleKey GetUserMove()
        {
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
                    return ++currentMouseX < this.Grid.GetLength(0);
                case ConsoleKey.UpArrow:
                    return --currentMouseY >= 0;
                case ConsoleKey.DownArrow:
                    return ++currentMouseY < this.Grid.GetLength(1);
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
            else if (mouseAfterMove.Status == Point.PointStatus.Cat || mouseAfterMove.Status == Point.PointStatus.CatAndCheese)
            {
                //clearing the old status from Mouse to Empty
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                //move Mouse to new position
                this.Mouse.Position = mouseAfterMove;
                //change the new position from status Cheese to Mouse
                this.Mouse.Position.Status = Point.PointStatus.Cat;

                this.Mouse.HasBeenPouncedOn = true;
                return false;
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

        /// <summary>
        /// Handles the logic for chasing the Mouse
        /// </summary>
        /// <param name="cat">Cat object</param>
        public void MoveCat(Cat cat)
        {
            //80% chance Cat will move
            if (8 >= this.RNG.Next(11))
            {
                //if Cat moves get the diffence between its position and the Mouse's
                int XpositionDif = this.Mouse.Position.X - cat.Position.X;
                int YpositionDif = this.Mouse.Position.Y - cat.Position.Y;

                //check to see their spacial relationship
                bool tryLeft = XpositionDif < 0;
                bool tryRight = XpositionDif > 0;
                bool tryUp = YpositionDif < 0;
                bool tryDown = YpositionDif > 0;

                //the target position of the Cat's move
                Point targetPosition = cat.Position;
                //bool variable to remaining in while loop to check if the Cat's move is valid
                bool validMove = false;

                while (!validMove && (tryLeft || tryRight || tryUp || tryDown))
                {
                    int targetX = cat.Position.X;
                    int targetY = cat.Position.Y;

                    //decide on which move for the Cat to make
                    if (tryRight)
                    {
                        targetPosition = Grid[++targetX, targetY];
                        tryRight = false;
                    }
                    else if (tryLeft)
                    {
                        targetPosition = Grid[--targetX, targetY];
                        tryLeft = false;
                    }
                    else if (tryDown)
                    {
                        targetPosition = Grid[targetX, ++targetY];
                        tryDown = false;
                    }
                    else if (tryUp)
                    {
                        targetPosition = Grid[targetX, --targetY];
                        tryUp = false;
                    }
                    //verify if the move is valid (determined by IsValidCatMove)
                    validMove = IsValidCatMove(targetPosition);
                }
                //once a valid move is found, change Cat's current position Status
                if (cat.Position.Status == Point.PointStatus.CatAndCheese)
                {
                    cat.Position.Status = Point.PointStatus.Cheese;
                }
                else
                {
                    cat.Position.Status = Point.PointStatus.Empty;
                }
                //change Cat's target position to a new Status depending on what's in that spot
                if (targetPosition.Status == Point.PointStatus.Mouse)
                {
                    this.Mouse.HasBeenPouncedOn = true;
                    targetPosition.Status = Point.PointStatus.Cat;
                }
                else if (targetPosition.Status == Point.PointStatus.Cheese)
                {
                    targetPosition.Status = Point.PointStatus.CatAndCheese;
                }
                else
                {
                    targetPosition.Status = Point.PointStatus.Cat;
                }
                //move the Cat
                cat.Position = targetPosition;
            }
        }

        /// <summary>
        /// Handles the logic for moving the Cat
        /// </summary>
        /// <param name="targetLocation">cell to move Cat to</param>
        /// <returns>true is cell is valid</returns>
        private bool IsValidCatMove(Point targetLocation)
        {
            //ensure that Cat does move to a spot already occupied by a Cat or CatandCheese
            return (targetLocation.Status == Point.PointStatus.Empty || targetLocation.Status == Point.PointStatus.Mouse || targetLocation.Status == Point.PointStatus.Cheese);
        }

        /// <summary>
        /// Handles the logic for playing the game
        /// </summary>
        public void PlayGame()
        {
            //play game while the Mouse has Energy
            while (this.Mouse.Energy > 0)
            {
                //draw the Grid
                this.DrawGrid();
                //get valid user input, if so move the Mouse
                this.MoveMouse(this.GetUserMove());
                //move each Cat in the CatList
                foreach (Cat cat in this.CatList)
                {
                    this.MoveCat(cat);
                }
                //increase Round count each play
                this.Round++;
                //if the Cat finds the Mouse it's Energy goes to 0 (Mouse dies)
                if (this.Mouse.HasBeenPouncedOn)
                {
                    this.Mouse.Energy = 0;
                }
            }
            //when game ends redraw the Grid to show results
            this.DrawGrid();
            Console.WriteLine("Game Over");
            Console.WriteLine("Number of Rounds: {0}", this.Round);
            //ask player if they want to play again
            this.PlayAgain();
        }

        /// <summary>
        /// Handles the logic for allowing user to play again or not
        /// </summary>
        public void PlayAgain()
        {
            Console.WriteLine();
            Console.WriteLine("Do you want to play again? Y or N");
            //if user chooses to play again
            if (Console.ReadLine().ToUpper() == "Y")
            {
                //reset all values to their defaults
                this.Mouse.HasBeenPouncedOn = false;
                this.Mouse.Energy = 50;
                this.CheeseCount = 0;
                this.Round = 0;
                //delete current Mouse, Cheese, and Cats
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                this.Cheese.Status = Point.PointStatus.Empty;
                foreach (Cat cat in this.CatList)
                {
                    cat.Position.Status = Point.PointStatus.Empty;
                }
                //reset the Mouse, Cheese, and CatList
                this.Mouse.Position = this.Grid[this.RNG.Next(10), this.RNG.Next(10)];
                this.Mouse.Position.Status = Point.PointStatus.Mouse;
                this.PlaceCheese();
                CatList.Clear();
                //restart Game
                this.PlayGame();
            }
        }
    }
}
