using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace NgeeAnnCity
{
    public class Arcade
    {
        private int profit;
        private Board board;
        private int turn;
        private int coins;
        private int points;
        private string[] buildings;
        private Random random;

        public Arcade()
        {
            board = new Board(20);
            turn = 0;
            coins = 16;
            points = 0;
            buildings = new string[] { "Residential", "Industry", "Commercial", "Park", "Road" };
            random = new Random();
        }

        public void Start()
        {
            board.Initialize();
            PlayGame();
        }

        private void PlayGame()
        {
            while (coins > 0)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                turn++;
                board.Display();
                DisplayStats();

                // Select two random buildings
                List<string> selectedBuildings = SelectBuildings();

                // Get player choice
                string chosenBuilding = GetPlayerChoice(selectedBuildings);
                char buildingSymbol = GetBuildingSymbol(chosenBuilding);

                // Place the chosen building
                board.PlaceBuilding(buildingSymbol, false);

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
                "Park" => 'O',
                "Road" => '*',
                _ => '.',
            };
        }
        private void DisplayStats()
        {
            Console.WriteLine(new string('-', 10) + "ARCADE MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins left: {coins}");
            Console.WriteLine($"Points: {points}");
        }


        private void UpdateScoresAndFinances()
        {
            points = 0;
            profit = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    char building = board.GetBuilding(i, j);
                    if (building != '.')
                    {
                        switch (building)
                        {
                            case 'R':
                                points += CalculateResidentialScore(i, j);
                                break;
                            case 'I':
                                points += CalculateIndustryScore();
                                profit += CountAdjacent(i, j, 'R');
                                break;
                            case 'C':
                                points += CalculateCommercialScore(i, j);
                                profit += CountAdjacent(i, j, 'R');
                                break;
                            case 'O':
                                points += CalculateParkScore(i, j);
                                break;
                            case '*':
                                points += CalculateRoadScore(i);
                                break;
                        }
                    }
                }
            }
            coins += profit;
        }

        private bool IsConnectedToOtherRoad(int row, int col, bool[][] visited)
        {
            if (row < 0 || row >= 20 || col < 0 || col >= 20 || visited[row][col] || board.GetBuilding(row, col) != '*')
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
            int points = 0;
            if (IsAdjacentTo(row, col, 'I'))
            {
                return 1;
            }
            points += CountAdjacent(row, col, 'R') + CountAdjacent(row, col, 'C') + 2 * CountAdjacent(row, col, 'O');
            return points;
        }
        private int CountAdjacentRoads(int row, int col)
        {
            int count = 0;
            if (row > 0 && board.GetBuilding(row - 1, col) == '*') count++; // Up
            if (row < 20 - 1 && board.GetBuilding(row + 1, col) == '*') count++; // Down
            if (col > 0 && board.GetBuilding(row, col - 1) == '*') count++; // Left
            if (col < 20 - 1 && board.GetBuilding(row, col + 1) == '*') count++; // Right
            return count;
        }

        private int CalculateIndustryScore()
        {
            int industryCount = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (board.GetBuilding(i, j) == 'I')
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
            int max = 0;
            int temp = 0;
            int roadScore = -1;
            for (int col = 0; col < 20; col++)
            {
                if (board.GetBuilding(row, col) == '*')
                {
                    temp++;
                } else if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

        private bool IsAdjacentTo(int row, int col, char building)
        {
            // 19 0
            return IsOrthogonallyAdjacent(row, col, building) || IsConnectedViaRoad(row, col);
        }

        private bool IsOrthogonallyAdjacent(int row, int col, char building)
        {
            return (row > 0 && board.GetBuilding(row - 1, col) == building) ||  //Up
                   (row < 19 && board.GetBuilding(row + 1, col) == building) || //Down
                   (col > 0 && board.GetBuilding(row, col - 1) == building) ||  //Left
                   (col < 19 && board.GetBuilding(row, col + 1) == building);   //Right
        }


        private bool IsDiagonallyAdjacent(int row, int col)
        {
            return (row > 0 && col > 0 && board.GetBuilding(row - 1, col - 1) != '.') || // Up-Left
                   (row > 0 && col < 20 - 1 && board.GetBuilding(row - 1, col + 1) != '.') || // Up-Right
                   (row < 20 - 1 && col > 0 && board.GetBuilding(row + 1, col - 1) != '.') || // Down-Left
                   (row < 20 - 1 && col < 20 - 1 && board.GetBuilding(row + 1, col + 1) != '.'); // Down-Right
        }

        private bool IsConnectedViaRoad(int row, int col)
        {
            bool[][] visited = new bool[20][];
            for (int i = 0; i < 20; i++)
            {
                visited[i] = new bool[20];
            }

            return IsConnectedViaRoadRec(row, col, visited, true);
        }

        private bool IsConnectedViaRoadRec(int row, int col, bool[][] visited, bool first = false)
        {

            if (row < 0 || row >= 20 || col < 0 || col >= 20 || visited[row][col] || first)
            {
                return false;
            }

            visited[row][col] = true;

            // checks for buildings that are not road
            if (board.GetBuilding(row, col) != '*' && board.GetBuilding(row, col) != '.' )
            {
                return true;  // Found any building
            }

            // check for nothing
            if (board.GetBuilding(row, col) != '*')
            {
                return false;  // No road found
            }

            // if it's road, run these checks
            return IsConnectedViaRoadRec(row - 1, col, visited) ||
                   IsConnectedViaRoadRec(row + 1, col, visited) ||
                   IsConnectedViaRoadRec(row, col - 1, visited) ||
                   IsConnectedViaRoadRec(row, col + 1, visited);
        }


        private bool IsAdjacent(int row, int col, char ignoreBuilding)
        {
            return (row > 0 && board.GetBuilding(row - 1, col) != ignoreBuilding) ||
                   (row < 19 && board.GetBuilding(row + 1, col) != ignoreBuilding) ||
                   (col > 0 && board.GetBuilding(row, col - 1) != ignoreBuilding) ||
                   (col < 19 && board.GetBuilding(row, col + 1) != ignoreBuilding);
        }

        private int CountAdjacent(int row, int col, char building)
        {
            int count = 0;
            if (row > 0 && board.GetBuilding(row - 1, col) == building) count++; // check left 
            if (row < 19 && board.GetBuilding(row + 1, col) == building) count++; // check right
            if (col > 0 && board.GetBuilding(row, col - 1) == building) count++; // check bottom
            if (col < 19 && board.GetBuilding(row, col + 1) == building) count++; // check left
            return count;
        }
    }
}