using System;
using System.Collections.Generic;

namespace NgeeAnnCity
{
    public class Arcade
    {
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
            coins = 100;
            points = 0;
            buildings = new string[] { "Residential", "Industry", "Commercial", "Park", "Road" } ;
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
                board.PlaceBuilding(buildingSymbol);

                // Update game state *not implemented yet*
                //CalculatePointsAndCoins();
                //CalculateUpkeepCosts();

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

        private void DisplayStats()
        {
            Console.WriteLine(new string('-', 10) + "ARCADE MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins left: {coins}");
            Console.WriteLine($"Points: {points}");
        }

        private void CalculatePointsAndCoins()
        {
            // Placeholder for actual scoring logic
            points += 5; // Example increment
            coins += 5;  // Example increment
        }

        private void CalculateUpkeepCosts()
        {
            // Placeholder for actual upkeep cost calculation
            coins -= 10; // Example decrement
        }
    }
}
