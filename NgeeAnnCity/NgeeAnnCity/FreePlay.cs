using System.ComponentModel;

namespace NgeeAnnCity
{
    class FreePlayGame
    {
        private const int InitialGridSize = 5;
        private char[,] grid;
        private int score;
        private int profit;
        private int upkeep;

        public FreePlayGame()
        {
            score = 0;
            profit = 0;
            upkeep = 0;
        }

        public void Start()
        {
            InitializeGrid();
            PlayGame();
        }

        private void InitializeGrid()
        {
            grid = new char[InitialGridSize, InitialGridSize];
            for (int i = 0; i < InitialGridSize; i++)
            {
                for (int j = 0; j < InitialGridSize; j++)
                {
                    grid[i, j] = '.';
                }
            }
        }

        public void PlayGame()
        {
            int turn = 1;
            while (true)
            {
                Console.Clear();
                DisplayGrid();
                DisplayInfo(turn);

                Console.WriteLine("Choose a building to construct (R, I, C, O, *): ");
                char choice = Console.ReadKey().KeyChar;
                choice = Char.ToUpper(choice);  //accepts any letter case.
                Console.WriteLine();

                if ("RICO*".Contains(choice))   //filters out any char that is NOT 'RICO*'
                {
                    bool buildingStatus = PlaceBuilding(choice);
                    if(buildingStatus)  //ensures number of turns doesn't increase even if user inputs an invalid row and column integer
                    {
                        UpdateScoresAndFinances();  //update score and profit/upkeep
                        turn++;
                    }
                    else { continue; }
                }
                else     //handle invalid user input
                {
                    Console.WriteLine("Invalid choice, try again.");
                    Console.ReadKey();
                }
            }
        }

        private bool PlaceBuilding(char building)
        {
            Console.WriteLine("Enter the row (1-5) and column (1-5) to place the building:");
            Console.Write("Row: ");
            string? rowInput = Console.ReadLine();
            Console.Write("Column: ");
            string? colInput = Console.ReadLine();
            if (int.TryParse(rowInput, out int row) && int.TryParse(colInput, out int col))//check if inputs can be parsed as ints, return integers if true;
            {
                row -= 1;
                col -= 1;
                if (row >= 0 && row < InitialGridSize && col >= 0 && col < InitialGridSize && grid[row, col] == '.')
                {
                    grid[row, col] = building;
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid location or cell already occupied. Try again.");
                    Console.ReadKey();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Please input an integer for the grid's row and column");
                Console.ReadKey();
                return false;
            }
        }

        private void UpdateScoresAndFinances()
        {
            score = 0;
            profit = 0;
            upkeep = 0;

            for (int i = 0; i < InitialGridSize; i++)
            {
                for (int j = 0; j < InitialGridSize; j++)
                {
                    char building = grid[i, j];
                    if (building != '.')
                    {
                        switch (building)
                        {
                            case 'R':
                                score += CalculateResidentialScore(i, j);
                                profit += 1;
                                //UPKEEP TO BE IMPLEMENTED LATER
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
                                if (!IsAdjacentTo(i, j, 'R') && !IsAdjacentTo(i, j, 'I') &&
                                    !IsAdjacentTo(i, j, 'C') && !IsAdjacentTo(i, j, 'O') &&
                                    !IsAdjacentTo(i, j, '*'))   //checks if road is NOT adjacent to any other buildings
                                {
                                    upkeep += 1;
                                }
                                break;
                        }
                    }
                }
            }
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

        private int CalculateIndustryScore()
        {
            int industryCount = 0;
            for (int i = 0; i < InitialGridSize; i++)
            {
                for (int j = 0; j < InitialGridSize; j++)
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
            for (int col = 0; col < InitialGridSize; col++)
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
            return (row > 0 && grid[row - 1, col] == building) ||
                   (row < InitialGridSize - 1 && grid[row + 1, col] == building) ||
                   (col > 0 && grid[row, col - 1] == building) ||
                   (col < InitialGridSize - 1 && grid[row, col + 1] == building);
        }

        private int CountAdjacent(int row, int col, char building)
        {
            int count = 0;
            if (row > 0 && grid[row - 1, col] == building) count++;
            if (row < InitialGridSize - 1 && grid[row + 1, col] == building) count++;
            if (col > 0 && grid[row, col - 1] == building) count++;
            if (col < InitialGridSize - 1 && grid[row, col + 1] == building) count++;
            return count;
        }
        private void DisplayGrid()
        {
            Console.WriteLine("Current Map:");
            for (int i = 0; i < InitialGridSize; i++)
            {
                for (int j = 0; j < InitialGridSize; j++)
                {
                    Console.Write(grid[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void DisplayInfo(int turn)
        {
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Score: {score}");
            Console.WriteLine($"Profit: {profit}");
            Console.WriteLine($"Upkeep: {upkeep}");
        }
    }
}