using System;
using System.Collections.Generic;

namespace NgeeAnnCity
{
    public class Arcade
    {
        private int height;
        private int width;
        private char[,] board;
        private int turnNumber;
        private int coins;
        private int points;
        private string[] buildings;
        private Random random;

        public Arcade()
        { 
            this.height = 20;
            this.width = 20;
            this.board = new char[height, width];
            this.turnNumber = 0;
            this.coins = 100;
            this.points = 0;
            this.buildings = new string[] { "Residential", "Industry", "Commercial", "Park", "Road" } ;
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
                    board[i, j] = '.';
                }
            }
        }

        private void PlayGame()
        {
            while (coins > 0)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                turnNumber++;
                DisplayBoard();
                DisplayStats();

                // Select two random buildings
                List<string> selectedBuildings = SelectBuildings();

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
                    break;
                }

                // check if spot is taken
                if (board[x, y] != '.')
                {
                    Console.WriteLine("Spot taken.\n");
                }
                else
                {
                    board[x, y] = building;
                    break;
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
                    Console.Write(board[i, j] + " ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }

        private void DisplayStats()
        {
            Console.WriteLine(new string('-', 10) + "ARCADE MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turnNumber}");
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
            for (int col = 0; col < width; col ++)
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
