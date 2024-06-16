using System;
using System.Collections.Generic;

namespace Arcade1
{
    public class Arcade
    {
        static Random random = new Random();
        static char[,] board = new char[20, 20];
        static int turnNumber = 0;
        static int coins = 100;
        static int points = 0;
        static List<string> buildings = new List<string> { "Residential", "Industry", "Commercial", "Park", "Road" };

        public void Start()
        {
            InitializeBoard();
            PlayGame();
        }

        static void InitializeBoard()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    board[i, j] = '.';
                }
            }
        }

        static void PlayGame()
        {
            while (coins > 0)
            {
                Console.Clear();
                turnNumber++;
                DisplayBoard();
                DisplayStats();

                // Select two random buildings
                List<string> selectedBuildings = SelectBuildings();
                DisplaySelectedBuildings(selectedBuildings);

                // Get player choice
                string chosenBuilding = GetPlayerChoice(selectedBuildings);
                char buildingSymbol = GetBuildingSymbol(chosenBuilding);

                // Place the chosen building
                PlaceBuilding(buildingSymbol);

                // Update game state *not implemented yet*
                //CalculatePointsAndCoins();
                //CalculateUpkeepCosts();

                Console.WriteLine("Press any key for the next turn...");
                Console.ReadKey();
            }
            Console.WriteLine("Game over! You've run out of coins.");
        }

        static List<string> SelectBuildings()
        {
            List<string> selectedBuildings = new List<string>();
            while (selectedBuildings.Count < 2)
            {
                string building = buildings[random.Next(buildings.Count)];
                if (!selectedBuildings.Contains(building))
                {
                    selectedBuildings.Add(building);
                }
            }
            return selectedBuildings;
        }

        static string GetPlayerChoice(List<string> selectedBuildings)
        {
            Console.WriteLine("Choose a building to construct:");
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

        static char GetBuildingSymbol(string building)
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

        static void PlaceBuilding(char building)
        {
            int x, y;
            while (true)
            {
                Console.WriteLine("Enter the row (0-19) to place the building:");
                x = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter the column (0-19) to place the building:");
                y = int.Parse(Console.ReadLine());
                if (x < 0 || x >= 20 || y < 0 || y >= 20)
                {
                    Console.WriteLine("Invalid position! Please enter valid row and column (0-19).");
                    continue;
                }

                else if (board[x, y] != '.')
                {
                    Console.WriteLine("Spot taken! Try another.");
                }
                else
                {
                    board[x, y] = building;
                    break;
                }
            }
        }

        static void DisplayBoard()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        static void DisplayStats()
        {
            Console.WriteLine($"Turn: {turnNumber}");
            Console.WriteLine($"Coins left: {coins}");
            Console.WriteLine($"Points: {points}");
        }

        static void DisplaySelectedBuildings(List<string> selectedBuildings)
        {
            Console.WriteLine("Selected buildings for this turn:");
            foreach (string building in selectedBuildings)
            {
                Console.WriteLine(building);
            }
        }

        static void CalculatePointsAndCoins()
        {
            // Placeholder for actual scoring logic
            points += 5; // Example increment
            coins += 5;  // Example increment
        }

        static void CalculateUpkeepCosts()
        {
            // Placeholder for actual upkeep cost calculation
            coins -= 10; // Example decrement
        }
    }
}
