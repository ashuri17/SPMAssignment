using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NgeeAnnCity
{
    public class Board
    {
        public int size {get; set;}
        private char[,] grid;
        public List<List<char>> gridList 
        {
            get{return gridList = ToNestedList(grid);}
            set{grid = ToCharArray(gridList);}
        }
        private Dictionary<Point, char> buildingDict;
        private int startRow = 0;
        private int startCol = 0;
        private int maxScreenSize;
        private const int expansionSize = 5;
      
        public List<KeyValuePair<Point, char>> serializedBuildingDict
            {
          get { return buildingDict.ToList(); }
          set { buildingDict= value.ToDictionary(x => x.Key, x => x.Value); }
            }

        public Board(int size, int maxScreenSize)
        {
            this.size = size;
            this.maxScreenSize = maxScreenSize;
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
        internal void DisplayBoard()
        {
            int width;
            Boolean canPanLeft = false;
            Boolean canPanRight = false;

            if (size < maxScreenSize)
            {
                width = size;
            }
            else
            {
                width = maxScreenSize;
            }
            int horizontalPadding = (startRow + width).ToString().Length;
            int verticalPadding = (startCol + width).ToString().Length;

            // check if user can pan up
            if (startRow != 0)
            {
                Console.Write(new String(' ', horizontalPadding + 5));
                Console.WriteLine(CenterString("^", width * 2 - 1));
                Console.WriteLine();
            }

            // check if user can pan left
            if (startCol != 0)
            {
                canPanLeft = true;
            }

            // check if user can pan right
            if (startCol + width != size)
            {
                canPanRight = true;
            }

            // get grid labels
            char[][] gridLabels = GetGridLabels(startCol + 1, width);

            // print top grid labels
            for (int i = 0; i < gridLabels.Length; i++)
            {
                Console.Write(new String(' ', horizontalPadding + 5));
                Console.WriteLine(string.Join(" ", gridLabels[i]));
            }

            Console.WriteLine();

            // print grid
            for (int i = startRow; i < startRow + width; i++)
            {
                // check when loop is in the middle so that indicator is centered
                int middle = (int) Math.Floor((decimal) (startRow + startRow + width) / 2);

                if (i == middle && canPanLeft)
                {
                    Console.Write(" < ");   
                } 
                else
                {
                    Console.Write("   ");
                }

                // grid label on the left
                Console.Write($"{i + 1}".PadLeft(horizontalPadding) + "  ");
              
                for (int j = startCol; j < startCol + width; j++)
                {
                    SetForegroundColor(grid[i, j]);
                    Console.Write(grid[i, j] + " ");
                }


                if (i == middle && canPanRight)
                {
                    Console.Write(" > "); 
                }
                Console.WriteLine();
            }

            // check if user can pan down
            if (startRow + width != size)
            {
                Console.WriteLine();
                Console.Write(new String(' ', horizontalPadding + 5));
                Console.WriteLine(CenterString("v", width * 2 - 1));
            }
            Console.WriteLine("\n\n");
        }

        private void SetForegroundColor(char building)
        {
            switch (building)
            {
                case 'R':
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 'I':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 'C':
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case 'O':
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case '*':
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
        }
        private List<List<char>> ToNestedList(char[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            List<List<char>> nestedList = new();

            for (int i = 0; i < rows; i++)
            {
                List<char> innerList = new();
                for (int j = 0; j < cols; j++)
                {
                    innerList.Add(array[i, j]);
                }
                nestedList.Add(innerList);
            }
            return nestedList;
        }
        private char[,] ToCharArray(List<List<char>> nestedList)
        {
            int rows = nestedList.Count;
            int cols = nestedList[0].Count; // Assuming all inner lists have the same length

            char[,] array = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = nestedList[i][j];
                }
            }
            return array;
        }

        private char[][] GetGridLabels(int startCol = 1, int width = 20) 
        {
            int endCol = startCol + width - 1; // Adjust to include the correct range

            // Convert each number to a string and find the maximum length
            string[] numberStrings = Enumerable.Range(startCol, width).Select(i => i.ToString()).ToArray();
            int maxLength = numberStrings.Max(s => s.Length); // Find the max length for padding

            // Initialize the rows array with the maximum length found
            char[][] rows = new char[maxLength][];

            // Initialize each row with spaces to ensure empty spaces are correctly formatted
            for (int i = 0; i < maxLength; i++)
            {
                rows[i] = Enumerable.Repeat(' ', width).ToArray();
            }

            // Iterate through each column label
            for (int col = 0; col < width; col++)
            {
                string num = numberStrings[col];
                int numLen = num.Length;

                // Place each digit of the number into the rows, starting from the bottom
                for (int digitIndex = 0; digitIndex < numLen; digitIndex++)
                {
                    // Calculate the row index (start from the bottom)
                    int rowIndex = maxLength - numLen + digitIndex;
                    rows[rowIndex][col] = num[digitIndex];
                }
            }
            return rows;
        }
        internal int GetExpansionSize()
        {
            return expansionSize;
        }
        internal void PlaceBuilding(char building, int x, int y)
        {
            grid[x, y] = building;
            StoreBuilding(building, x, y);  
        }
        internal void DemolishBuilding()
        {
            int x, y;
            //runs until a building is demolished
            while (true)
            {
                //get row from user
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
                // check if spot is empty
                if (grid[x - 1, y - 1] == '.')
                {
                    Console.WriteLine("No building to demolish.\n");
                }
                else
                {
                    grid[x - 1, y - 1] = Convert.ToChar(".");
                    RemoveBuilding(x - 1, y - 1);
                    break;
                }
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
        internal void RemoveBuilding(int row, int column)
        {
            buildingDict.Remove(new Point(row, column));
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
        internal int GetMaxScreenSize()
        {
            return maxScreenSize;
        }
        internal bool IsAdjacentToExistingBuilding(int row, int col)
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
        internal bool IsAdjacentTo(int row, int col, char building)
        {

            return IsOrthogonallyAdjacent(row, col, building) || IsDiagonallyAdjacentTo(row, col, building);
        }
        internal bool FreePlayIsAdjacentTo(int row, int col, char building)
        {

            return IsOrthogonallyAdjacent(row, col, building) || IsDiagonallyAdjacentTo(row, col, building) || IsConnectedViaRoadSpec(row, col, building);
        }
        internal bool IsOrthogonallyAdjacent(int row, int col, char building)
        {
            return (row > 0 && GetBuilding(row - 1, col) == building) ||  //Up
                   (row < size - 1 && GetBuilding(row + 1, col) == building) || //Down
                   (col > 0 && GetBuilding(row, col - 1) == building) ||  //Left
                   (col < size - 1 && GetBuilding(row, col + 1) == building);   //Right
        }
        internal bool IsDiagonallyAdjacentTo(int row, int col, char building)
        {
            return (row > 0 && col > 0 && GetBuilding(row - 1, col - 1) == building) || // Up-Left
                   (row > 0 && col < size - 1 && GetBuilding(row - 1, col + 1) == building) || // Up-Right
                   (row < size - 1 && col > 0 && GetBuilding(row + 1, col - 1) == building) || // Down-Left
                   (row < size - 1 && col < size - 1 && GetBuilding(row + 1, col + 1) == building); // Down-Right
        }
        internal int CountAdjacentRow(int row, int col, char building)
        {
            int count = 0;
            if (col > 0 && GetBuilding(row, col - 1) == building) count++; // check left
            if (col < size - 1 && GetBuilding(row, col + 1) == building) count++; // check right
            return count;
        }
        internal int CountAdjacent(int row, int col, char building)
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
        internal int CountAdjacentFreePlay(int row, int col, char building)
        {
            int count = 0;
            bool[][] visited = new bool[size][];
            for (int i = 0; i < size; i++)
            {
                visited[i] = new bool[size];
            }
            // Check orthogonal directions
            if (row > 0 && GetBuilding(row - 1, col) == building) { visited[row - 1][col] = true; count++; } // check up
            if (row < size - 1 && GetBuilding(row + 1, col) == building) { visited[row + 1][col] = true; count++; } // check down
            if (col > 0 && GetBuilding(row, col - 1) == building) { visited[row][col - 1] = true; count++; } // check left
            if (col < size - 1 && GetBuilding(row, col + 1) == building) { visited[row][col + 1] = true; count++; } // check right

            // Check diagonal directions
            if (row > 0 && col > 0 && GetBuilding(row - 1, col - 1) == building) { visited[row - 1][col - 1] = true; count++; } // check up-left
            if (row > 0 && col < size - 1 && GetBuilding(row - 1, col + 1) == building) { visited[row - 1][col + 1] = true; count++; } // check up-right
            if (row < size - 1 && col > 0 && GetBuilding(row + 1, col - 1) == building) { visited[row + 1][col - 1] = true; count++; } // check down-left
            if (row < size - 1 && col < size - 1 && GetBuilding(row + 1, col + 1) == building) { visited[row + 1][col + 1] = true; count++; } // check down-right

            // Check if connected via road
            count += CountConnectedViaRoad(row, col, size, building, visited);
            return count;
        }
        internal int CountConnectedViaRoad(int row, int col, int size, char building, bool[][] visited)
        {
            int callCount = 1;
            // Skip the first building and start counting in the recursive function
            return CountConnectedViaRoadRec(row, col, visited, building, size, callCount);
        }
        private int CountConnectedViaRoadRec(int row, int col, bool[][] visited, char building, int size, int callCount)
        {
            callCount += 1;
            if (row < 0 || row >= size || col < 0 || col >= size || visited[row][col])
            {
                return 0;
            }
            visited[row][col] = true;

            int foundCount = 0;

            // Skip counting the building in the first call
            if (callCount > 2 && grid[row, col] == building)
            {
                foundCount = 1;  // Found specified building
                return foundCount;
            }

            // Continue recursion only if the current cell is a road or the first cell
            if (callCount == 2 || grid[row, col] == '*')
            {
                // Sum the results of the recursive calls
                foundCount += CountConnectedViaRoadRec(row - 1, col, visited, building, size, callCount); //Up
                foundCount += CountConnectedViaRoadRec(row + 1, col, visited, building, size, callCount); //Down
                foundCount += CountConnectedViaRoadRec(row, col - 1, visited, building, size, callCount); //Left
                foundCount += CountConnectedViaRoadRec(row, col + 1, visited, building, size, callCount); //Right
                foundCount += CountConnectedViaRoadRec(row - 1, col - 1, visited, building, size, callCount); // Up-Left
                foundCount += CountConnectedViaRoadRec(row - 1, col + 1, visited, building, size, callCount); // Up-Right
                foundCount += CountConnectedViaRoadRec(row + 1, col - 1, visited, building, size, callCount); // Down-Left
                foundCount += CountConnectedViaRoadRec(row + 1, col + 1, visited, building, size, callCount); // Down-Right
            }

            return foundCount;
        }
        internal bool IsConnectedViaRoadSpec(int row, int col, char building)
        {
            int count = 0; // to skip the first recursion, the starting cell
            bool[][] visited = new bool[size][];
            for (int i = 0; i < size; i++)
            {
                visited[i] = new bool[size];
            }

            return IsConnectedViaRoadRecSpec(row, col, visited, count, building);
        }
        internal bool IsConnectedViaRoadRecSpec(int row, int col, bool[][] visited, int count, char building)
        {
            if (row < 0 || row >= size || col < 0 || col >= size || visited[row][col])
            {
                return false;
            }

            visited[row][col] = true;

            if (grid[row, col] == building && count != 0)
            {
                return true;  // Found specified building
            }
            count += 1;

            if (count == 1 || grid[row, col] == '*')
            {
                return IsConnectedViaRoadRecSpec(row - 1, col, visited, count, building) ||    // Up
                   IsConnectedViaRoadRecSpec(row + 1, col, visited, count, building) ||    // Down
                   IsConnectedViaRoadRecSpec(row, col - 1, visited, count, building) ||    // Left
                   IsConnectedViaRoadRecSpec(row, col + 1, visited, count, building) ||    // Right
                   IsConnectedViaRoadRecSpec(row - 1, col - 1, visited, count, building) || // Up-Left
                   IsConnectedViaRoadRecSpec(row - 1, col + 1, visited, count, building) || // Up-Right
                   IsConnectedViaRoadRecSpec(row + 1, col - 1, visited, count, building) || // Down-Left
                   IsConnectedViaRoadRecSpec(row + 1, col + 1, visited, count, building);   // Down-Right
            }
            return false;
        }
        internal bool isGridFull()
        {
            return buildingDict.Count == size * size;    //board is full.
        }
        internal bool isGridEmpty()
        {
            return buildingDict.Count == 0;    //board does not contain a building.
        }
        internal void PanLeft()
        {
            startCol = startCol - maxScreenSize < 0 ? 0 : startCol - maxScreenSize;
        }
        internal void PanRight()
        {
            startCol = startCol + maxScreenSize > size - maxScreenSize ? size - maxScreenSize : startCol + maxScreenSize;
        }
        internal void PanUp()
        {
            startRow = startRow - maxScreenSize < 0 ? 0 : startRow - maxScreenSize;
        }
        internal void PanDown() 
        {
            startRow = startRow + maxScreenSize > size - maxScreenSize ? size - maxScreenSize : startRow + maxScreenSize;
        }
        internal void PanTo(int row, int col)
        {
            if (size <= maxScreenSize)
            {
                startRow = 0;
                startCol = 0;
                return;
            }

            int halfScreenSize = (int) (Math.Ceiling((decimal) maxScreenSize / 2));
            row++; // incremented because it was decremented for placebuilding();
            col++;

            startRow = row - halfScreenSize;
            startCol = col - halfScreenSize;

            if (startRow < 0)
            {
                startRow = 0;
            } 
            else if (startRow + maxScreenSize > size)
            {
                startRow = size - maxScreenSize;
            } 
            
            if (startCol < 0)
            {
                startCol = 0;
            } 
            else if (startCol + maxScreenSize > size) 
            {
                startCol = size - maxScreenSize;
            }
        }
        private static string CenterString(String s, int width)
        {
            string padding = new string(' ', (int)Math.Floor((double)(width - s.Length) / 2));
            return padding + s + padding;
        }

    }
}