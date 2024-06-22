using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace NgeeAnnCity
{
    public class Arcade
    {
        private int height;
        private int width;
        private char[,] grid;
        private int turn;
        private int coins;
        private int score;
        private int profit;
        private int upkeep;
        private string[] buildings;
        private Random random;

        public Arcade()
        {
            this.height = 20;
            this.width = 20;
            this.grid = new char[height, width];
            this.turn = 0;
            this.coins = 16;
            this.score = 0;
            this.buildings = new string[] { "Residential", "Industry", "Commercial", "Park", "Road" };
            this.random = new Random();
        }

        public void Start()
        {
            InitializeBoard();
            PlayGame();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    grid[i, j] = '.';
                }
            }
        }

        private void PlayGame()
        {
            while (coins > 0)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                turn++;
                DisplayBoard();
                DisplayStats();

                // Select two random buildings
                List<string> selectedBuildings = SelectBuildings();

                // Get player choice
                string chosenBuilding = GetPlayerChoice(selectedBuildings);
                char buildingSymbol = GetBuildingSymbol(chosenBuilding);

                // Place the chosen building
                PlaceBuilding(buildingSymbol);

                // Update game state
                coins -= 1;
                UpdateScoresAndFinances();
                Console.WriteLine("Press any key for the next turn...");
                Console.ReadKey();
            }
            Console.WriteLine("Game over! You've run out of coins.");
        }

        private List<string> SelectBuildings()
        {
            List<string> selectedBuildings = new List<string>();
            while (selectedBuildings.Count < 2)
            {
                string building = buildings[random.Next(buildings.Length)];
                if (!selectedBuildings.Contains(building))
                {
                    selectedBuildings.Add(building);
                }
            }
            return selectedBuildings;
        }

        private string GetPlayerChoice(List<string> selectedBuildings)
        {
            Console.WriteLine("Choose a building to construct (1/2):");
            for (int i = 0; i < selectedBuildings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {selectedBuildings[i]}");
            }

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > selectedBuildings.Count)
            {
                Console.WriteLine("Invalid choice. Please select 1 or 2:");
            }

            return selectedBuildings[choice - 1];
        }
        private char GetBuildingSymbol(string building)
        {
            return building switch
            {
                "Residential" => 'R',
                "Industry" => 'I',
                "Commercial" => 'C',
                "Park" => 'P',
                "Road" => '*',
                _ => '.',
            };
        }

        private void PlaceBuilding(char building)
        {
            int x, y;

            // runs until a building is placed
            while (true)
            {
                // get row from user
                while (true)
                {
                    Console.Write("Row (1-20): ");

                    // check if user enters a number that falls within the width of the board
                    if (!int.TryParse(Console.ReadLine(), out x) || x < 1 || x > 20)
                    {
                        Console.WriteLine("Invalid row.\n");
                        continue;
                    }
                    x = x - 1;
                    break;
                }

                // get column from user 
                while (true)
                {
                    Console.Write("Column (1-20): ");

                    // check if user enters a number that falls within the height of the board
                    if (!int.TryParse(Console.ReadLine(), out y) || y < 1 || y > 20)
                    {
                        Console.WriteLine("Invalid column.\n");
                        continue;
                    }
                    y = y - 1;
                    break;
                }

                // check if spot is taken
                if (grid[x, y] != '.')
                {
                    Console.WriteLine("Spot taken.\n");
                }
                else
                {
                    // Check if the spot is adjacent to an existing building
                    if (turn == 1 || IsAdjacent(x, y, '.')) // Allow placing on any spot for the first turn
                    {
                        grid[x, y] = building;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Building must be placed adjacent to an existing building.\n");
                    }
                }
            }
        }

        private void DisplayBoard()
        {
            int horizontalPadding = this.width.ToString().Length + 1;

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
            for (int i = 0; i < this.height; i++)
            {
                // grid label on the left
                Console.Write($"{i + 1}".PadLeft(horizontalPadding) + "  ");

                for (int j = 0; j < width; j++)
                {
                    Console.Write(grid[i, j] + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }

        private void DisplayStats()
        {
            Console.WriteLine(new string('-', 10) + "ARCADE MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins left: {coins}");
            Console.WriteLine($"Points: {score}");
        }

        private void UpdateScoresAndFinances()
        {
            score = 0;
            profit = 0;
            upkeep = 0;

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    char building = grid[i, j];
                    if (building != '.')
                    {
                        switch (building)
                        {
                            case 'R':
                                score += CalculateResidentialScore(i, j);
                                profit += 1;
                                if (!IsPartOfResidentialCluster(i, j))
                                {
                                    upkeep += 1;
                                }
                                break;
                            case 'I':
                                score += CalculateIndustryScore();
                                profit += 2;
                                upkeep += 1;
                                break;
                            case 'C':
                                score += CalculateCommercialScore(i, j);
                                profit += 3;
                                upkeep += 2;
                                break;
                            case 'O':
                                score += CalculateParkScore(i, j);
                                upkeep += 1;
                                break;
                            case '*':
                                score += CalculateRoadScore(i);
                                upkeep += CalculateRoadUpkeep();
                                break;
                        }
                    }
                }
            }
            coins += profit;
            coins -= upkeep;
        }
        private int CalculateRoadUpkeep()
        {
            int roadUpkeep = 0;
            bool[][] visited = new bool[20][];
            for (int i = 0; i < 20; i++)
            {
                visited[i] = new bool[20];
            }

            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 20; col++)
                {
                    if (grid[row, col] == '*' && !IsConnectedToOtherRoad(row, col, visited))
                    {
                        roadUpkeep++;
                    }
                }
            }

            return roadUpkeep;
        }

        private bool IsConnectedToOtherRoad(int row, int col, bool[][] visited)
        {
            if (row < 0 || row >= 20 || col < 0 || col >= 20 || visited[row][col] || grid[row, col] != '*')
            {
                return false;
            }

            visited[row][col] = true;

            // Check all orthogonal directions for connected road segments
            return IsConnectedToOtherRoad(row - 1, col, visited) ||
                   IsConnectedToOtherRoad(row + 1, col, visited) ||
                   IsConnectedToOtherRoad(row, col - 1, visited) ||
                   IsConnectedToOtherRoad(row, col + 1, visited);
        }
        private int CalculateResidentialScore(int row, int col)
        {
            int score = 0;
            if (IsAdjacentTo(row, col, 'I'))
            {
                return 1;
            }
            score += CountAdjacent(row, col, 'R') + CountAdjacent(row, col, 'C') + 2 * CountAdjacent(row, col, 'O');
            return score;
        }
        private bool IsPartOfResidentialCluster(int row, int col)
        {
            return (row > 0 && grid[row - 1, col] == 'R') || // Up
                   (row < 20 - 1 && grid[row + 1, col] == 'R') || // Down
                   (col > 0 && grid[row, col - 1] == 'R') || // Left
                   (col < 20 - 1 && grid[row, col + 1] == 'R') || // Right
                   (row > 0 && col > 0 && grid[row - 1, col - 1] == 'R') || // Up-Left
                   (row > 0 && col < 20 - 1 && grid[row - 1, col + 1] == 'R') || // Up-Right
                   (row < 20 - 1 && col > 0 && grid[row + 1, col - 1] == 'R') || // Down-Left
                   (row < 20 - 1 && col < 20 - 1 && grid[row + 1, col + 1] == 'R'); // Down-Right
        }

        private int CalculateIndustryScore()
        {
            int industryCount = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (grid[i, j] == 'I')
                    {
                        industryCount++;
                    }
                }
            }
            return industryCount;
        }

        private int CalculateCommercialScore(int row, int col)
        {
            return CountAdjacent(row, col, 'C');
        }

        private int CalculateParkScore(int row, int col)
        {
            return CountAdjacent(row, col, 'O');
        }

        private int CalculateRoadScore(int row)
        {
            int roadScore = 0;
            for (int col = 0; col < 20; col++)
            {
                if (grid[row, col] == '*')
                {
                    roadScore++;
                }
            }
            return roadScore;
        }

        private bool IsAdjacentTo(int row, int col, char building)
        {
            return IsOrthogonallyAdjacent(row, col) || IsConnectedViaRoad(row, col);
        }

        private bool IsOrthogonallyAdjacent(int row, int col)
        {
            return (row > 0 && grid[row - 1, col] != '.') ||//Up
                   (row < 20 - 1 && grid[row + 1, col] != '.') ||//Down
                   (col > 0 && grid[row, col - 1] != '.') ||//Left
                   (col < 20 - 1 && grid[row, col + 1] != '.');//Right
        }
        private bool IsDiagonallyAdjacent(int row, int col)
        {
            return (row > 0 && col > 0 && grid[row - 1, col - 1] != '.') || // Up-Left
                   (row > 0 && col < 20 - 1 && grid[row - 1, col + 1] != '.') || // Up-Right
                   (row < 20 - 1 && col > 0 && grid[row + 1, col - 1] != '.') || // Down-Left
                   (row < 20 - 1 && col < 20 - 1 && grid[row + 1, col + 1] != '.'); // Down-Right
        }

        private bool IsConnectedViaRoad(int row, int col)
        {
            bool[][] visited = new bool[20][];
            for (int i = 0; i < 20; i++)
            {
                visited[i] = new bool[20];
            }

            return IsConnectedViaRoadRec(row, col, visited);
        }

        private bool IsConnectedViaRoadRec(int row, int col, bool[][] visited)
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


        private bool IsAdjacent(int row, int col, char ignoreBuilding)
        {
            return (row > 0 && grid[row - 1, col] != ignoreBuilding) ||
                   (row < height - 1 && grid[row + 1, col] != ignoreBuilding) ||
                   (col > 0 && grid[row, col - 1] != ignoreBuilding) ||
                   (col < width - 1 && grid[row, col + 1] != ignoreBuilding);
        }

        private int CountAdjacent(int row, int col, char building)
        {
            int count = 0;
            if (row > 0 && grid[row - 1, col] == building) count++;
            if (row < 20 - 1 && grid[row + 1, col] == building) count++;
            if (col > 0 && grid[row, col - 1] == building) count++;
            if (col < 20 - 1 && grid[row, col + 1] == building) count++;
            return count;
        }

        private char[][] GetGridLabels()
        {
            // Get each number to print
            int[] numbers = Enumerable.Range(1, this.width).ToArray();

            // Convert each number to a string
            string[] numberStrings = numbers.Select(i => i.ToString()).ToArray();

            // Max number of digits for each number => number of digits the largest number has => number of digits the width has
            int maxLength = this.width.ToString().Length;

            // Number of rows the array should have
            char[][] rows = new char[maxLength][];

            // Number of columns (digits) each row should have
            for (int i = 0; i < maxLength; i++)
            {
                rows[i] = new char[this.width];
            }

            // Iterate through each number
            for (int col = 0; col < width; col++)
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
    }
}