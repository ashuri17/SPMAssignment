using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NgeeAnnCity
{
    internal class Board
    {
        private int size;
        private char[,] grid;
        private Dictionary<Point, char> buildingDict;
        const int expansionSize = 5;

        public Board(int size)
        {
            this.size = size;
            grid = new char[size, size];
            buildingDict = new Dictionary<Point, char>();
        }

        internal void Initialize()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = '.';
                }
            }
        }

        internal void Display()
        {
            int horizontalPadding = size.ToString().Length + 1;

            // get grid labels
            char[][] gridLabels = GetGridLabels();

            // print top grid labels
            for (int i = 0; i < gridLabels.Length; i++)
            {
                Console.Write(new String(' ', horizontalPadding + 2));
                Console.WriteLine(string.Join(" ", gridLabels[i]));
            }

            Console.WriteLine();

            // print grid
            for (int i = 0; i < size; i++)
            {
                // grid label on the left
                Console.Write($"{i + 1}".PadLeft(horizontalPadding) + "  ");

                for (int j = 0; j < size; j++)
                {
                    Console.Write(grid[i, j] + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }

        private char[][] GetGridLabels()
        {
            // Get each number to print
            int[] numbers = Enumerable.Range(1, size).ToArray();

            // Convert each number to a string
            string[] numberStrings = numbers.Select(i => i.ToString()).ToArray();

            // Max number of digits for each number => number of digits the largest number has => number of digits the width has
            int maxLength = size.ToString().Length;

            // Number of rows the array should have
            char[][] rows = new char[maxLength][];

            // Number of columns (digits) each row should have
            for (int i = 0; i < maxLength; i++)
            {
                rows[i] = new char[size];
            }

            // Iterate through each number
            for (int col = 0; col < size; col++)
            {
                string num = numberStrings[col];
                int numLen = num.Length;

                // Iterate through each digit
                for (int row = 0; row < maxLength; row++)
                {
                    // check if num has a digit in the row (99 doesn't have a digit in the first row if the width is 100) 
                    // e.g.
                    //
                    //        1
                    //   9    0
                    //   9    0

                    if (row < numLen)
                    {
                        // If width is 2 digits, there will be 2 rows.
                        // e.g.
                        // {...} <- stores the digit in the "tens" place
                        // {...} <- stores the digit in the "ones" place
                        //
                        // If num is 5, numLen is 1.
                        // During the first iteration of the for loop, the if statement will evaluate to (0 < 1).
                        // Since it is true, 5 will be added to the last row.
                        //
                        // If it is 25, 5 will be added to the last row.
                        // If it is 125, 5 will be added to the last row.
                        //
                        //
                        // Subsequent iterations will check the other digits and if possible, add them to the other rows.

                        rows[maxLength - row - 1][col] = num[numLen - row - 1];
                    }
                    else
                    {
                        // If there is no digits, represent as whitespace
                        rows[maxLength - row - 1][col] = ' ';
                    }
                }
            }
            return rows;
        }

        internal void PlaceBuilding(char building, int turn, bool freeplay = true)
        {
            int x, y;

            // runs until a building is placed
            while (true)
            {
                // get row from user
                while (true)
                {
                    Console.Write($"Row (1-{size}): ");

                    // check if user enters a number that falls within the width of the board
                    if (!int.TryParse(Console.ReadLine(), out x) || x < 1 || x > size)
                    {
                        Console.WriteLine("Invalid row.\n");
                        continue;
                    }
                    break;
                }

                // get column from user 
                while (true)
                {
                    Console.Write($"Column (1-{size}): ");

                    // check if user enters a number that falls within the height of the board
                    if (!int.TryParse(Console.ReadLine(), out y) || y < 1 || y > size)
                    {
                        Console.WriteLine("Invalid column.\n");
                        continue;
                    }
                    break;
                }

                x--;
                y--;

                // check if spot is taken
                if (grid[x, y] != '.')
                {
                    Console.WriteLine("Spot taken.\n");
                    continue;
                }
                //check if buildings in arcade are placed adjacent to existing buildings
                else if (!freeplay && turn > 1 && !IsAdjacentToExistingBuilding(x, y))
                {
                    Console.WriteLine("Building must be placed adjacent to an existing building.\n");
                    continue;
                }
                else
                {
                    grid[x, y] = building;
                    StoreBuilding(building, x, y);
                }

                if (freeplay)
                {
                    if (TouchingBorder(x, y))
                    {
                        ExpandGrid();
                    }
                }
                break;
            }
        }

        internal char GetBuilding(int row, int column)
        {
            return grid[row, column];
        }

        internal void StoreBuilding(char building, int row, int column)
        {
            buildingDict.Add(new Point(row, column), building);
        }

        internal bool TouchingBorder(int row, int column)
        {
            int maxIndex = size - 1;
            return (row == 0 || row == maxIndex || column == 0 || column == maxIndex);
        }

        internal void ExpandGrid()
        {
            size += expansionSize * 2;
            grid = new char[size, size];
            Initialize();
            foreach (var buildingInfo in buildingDict.ToList())
            {
                Point coords = buildingInfo.Key;
                char building = buildingInfo.Value;
                coords.X += 5;
                coords.Y += 5;
                grid[coords.X, coords.Y] = building;
                buildingDict.Remove(new Point(coords.X - 5, coords.Y - 5));
                buildingDict.Add(new Point(coords.X, coords.Y), building);
            }
        }

        internal int GetSize()
        {
            return size;
        }

        private bool IsAdjacentToExistingBuilding(int row, int col) // to check if arcade buildings are placed adjacent to existing buildings
        {
            // Check orthogonal directions
            if (row > 0 && grid[row - 1, col] != '.') return true; // check up
            if (row < size - 1 && grid[row + 1, col] != '.') return true; // check down
            if (col > 0 && grid[row, col - 1] != '.') return true; // check left
            if (col < size - 1 && grid[row, col + 1] != '.') return true; // check right

            // Check diagonal directions
            if (row > 0 && col > 0 && grid[row - 1, col - 1] != '.') return true; // check up-left
            if (row > 0 && col < size - 1 && grid[row - 1, col + 1] != '.') return true; // check up-right
            if (row < size - 1 && col > 0 && grid[row + 1, col - 1] != '.') return true; // check down-left
            if (row < size - 1 && col < size - 1 && grid[row + 1, col + 1] != '.') return true; // check down-right

            return false;
        }



        //arcade adjacent logic 
        public bool IsAdjacentTo(int row, int col, char building)
        {

            return IsOrthogonallyAdjacent(row, col, building) || IsDiagonallyAdjacentTo(row, col, building) || IsConnectedViaRoad(row, col);
        }

        public bool IsOrthogonallyAdjacent(int row, int col, char building) //a check function to check if buildings are orthogonally adjacent to a specific building type
        {
            return (row > 0 && GetBuilding(row - 1, col) == building) ||  //Up
                   (row < size-1 && GetBuilding(row + 1, col) == building) || //Down
                   (col > 0 && GetBuilding(row, col - 1) == building) ||  //Left
                   (col < size-1 && GetBuilding(row, col + 1) == building);   //Right
        }


        public bool IsDiagonallyAdjacentTo(int row, int col, char building) //a check function to check if buildings are diagonally adjacent to a specific building type
        {
            return (row > 0 && col > 0 && GetBuilding(row - 1, col - 1) == building) || // Up-Left
                   (row > 0 && col < size - 1 && GetBuilding(row - 1, col + 1) == building) || // Up-Right
                   (row < size - 1 && col > 0 && GetBuilding(row + 1, col - 1) == building) || // Down-Left
                   (row < size - 1 && col < size - 1 && GetBuilding(row + 1, col + 1) == building); // Down-Right
        }



        public int CountAdjacent(int row, int col, char building) // count the number of the specified building type adjacent to the current building
        {
            int count = 0;

            // Check orthogonal directions
            if (row > 0 && GetBuilding(row - 1, col) == building) count++; // check up
            if (row < size - 1 && GetBuilding(row + 1, col) == building) count++; // check down
            if (col > 0 && GetBuilding(row, col - 1) == building) count++; // check left
            if (col < size - 1 && GetBuilding(row, col + 1) == building) count++; // check right

            // Check diagonal directions
            if (row > 0 && col > 0 && GetBuilding(row - 1, col - 1) == building) count++; // check up-left
            if (row > 0 && col < size - 1 && GetBuilding(row - 1, col + 1) == building) count++; // check up-right
            if (row < size - 1 && col > 0 && GetBuilding(row + 1, col - 1) == building) count++; // check down-left
            if (row < size - 1 && col < size - 1 && GetBuilding(row + 1, col + 1) == building) count++; // check down-right

            return count;
        }
        public int CountAdjacentRow(int row, int col, char building) //check adjacent rows, mainly for road
        {
            int count = 0;
            if (col > 0 && GetBuilding(row, col - 1) == building) count++; // check left
            if (col < size - 1 && GetBuilding(row, col + 1) == building) count++; // check right
            return count;
        }

        public bool IsConnectedViaRoad(int row, int col)
        {
            bool[][] visited = new bool[20][];
            for (int i = 0; i < 20; i++)
            {
                visited[i] = new bool[20];
            }

            return IsConnectedViaRoadRec(row, col, visited);
        }

        public bool IsConnectedViaRoadRec(int row, int col, bool[][] visited)
        {
            if (row < 0 || row >= 20 || col < 0 || col >= 20 || visited[row][col])
            {
                return false;
            }

            visited[row][col] = true;

            if (grid[row, col] != '*' && grid[row, col] != '.')
            {
                return true;  // Found any building
            }

            if (grid[row, col] != '*')
            {
                return false;  // No road found
            }

            return IsConnectedViaRoadRec(row - 1, col, visited) ||
                   IsConnectedViaRoadRec(row + 1, col, visited) ||
                   IsConnectedViaRoadRec(row, col - 1, visited) ||
                   IsConnectedViaRoadRec(row, col + 1, visited);
        }



        public bool isGridFull()
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (GetBuilding(row, col) == '.')
                    {
                        return false;   //board contains an empty cell
                    }
                }
            }
            return true;    //board no longer has an empty cell for a building to be constructed
        }

    }
}