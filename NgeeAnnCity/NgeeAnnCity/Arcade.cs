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
            coins = 2;
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
            while (coins > 0 && !board.isGridFull())   //end game conditions
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
                board.PlaceBuilding(buildingSymbol,turn, false);

                // Update game state
                coins -= 1;
                UpdateScoresAndFinances();
                Console.WriteLine("Press any key for the next turn...");
                Console.ReadKey();
            }
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            DisplayEndGame();
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
                                points += 1;
                                profit += board.CountAdjacent(i, j, 'R'); // counts how many residential buildings are adjacent to it, to add the coins
                                break;
                            case 'C':
                                points += CalculateCommercialScore(i, j);
                                profit += board.CountAdjacent(i, j, 'R'); // counts how many residential buildings are adjacent to it, to add the coins
                                break;
                            case 'O':
                                points += CalculateParkScore(i, j); 
                                break;
                            case '*':
                                points += CalculateRoadScore(i,j);
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
            if (board.IsAdjacentTo(row, col, 'I')) // industry buildings only count once
            {
                points += 1;
            }
            points += board.CountAdjacent(row, col, 'R') + board.CountAdjacent(row, col, 'C') + 2 * board.CountAdjacent(row, col, 'O');
            return points;
        }
        private int CalculateCommercialScore(int row, int col)
        {
            return board.CountAdjacent(row, col, 'C');
        }

        private int CalculateParkScore(int row, int col)
        {
            return board.CountAdjacent(row, col, 'O');
        }

        private int CalculateRoadScore(int row,int col)
        {
            return board.CountAdjacentRow(row, col, '*');
        }

        //End Game Display
        private void DisplayEndGame()
        {
            Console.Clear();
            string[] lines = {
                 "GAME END",
                 $"Score: {points}",
                 $"Total Turns: {turn}",
                 "Press any key to continue..."
            };

            int width = lines.Max(line => line.Length) + 4;
            string horizontalBorder = new string('_', width);   //top border

            Console.WriteLine(horizontalBorder);
            //create box to display final info
            foreach (var line in lines)
            {
                int padding = width - line.Length - 2;
                string paddedLine = $"| {line}{new string(' ', padding)}|";     //adds | to each line in lines array
                Console.WriteLine(paddedLine);
            }
            Console.WriteLine(horizontalBorder);    //bottom border

            Console.ReadKey();
            EditHighScores();
        }


        private void EditHighScores()
        {
            List<(string name, int score)> highScoresList = new List<(string name, int score)>();

            string[] arcadeHighScores = File.ReadAllLines("arcadehighscores.csv");

            foreach (string line in arcadeHighScores)
            {
                string[] parts = line.Split(',');   //splits the playerName and points
                highScoresList.Add((parts[0], int.Parse(parts[1])));
            }

            if (points > highScoresList[^1].score)  //this checks if current player points is greater than the last score on the highscoreList
            {
                string? playerName = null;
                Console.WriteLine("Congrats on achieving a new high score!");
              
                //loops till playerName is no longer null
                while (string.IsNullOrEmpty(playerName))
                {
                    Console.Write("Please enter your name: ");
                    playerName = Console.ReadLine();
                }

                highScoresList.Add((playerName, points));
                highScoresList.Sort((x, y) => y.score.CompareTo(x.score));  //sorts list in descending order of score
                if (highScoresList.Count > 10)
                {
                    highScoresList.RemoveAt(10);    //ensures list only contains 10 entries
                }
                using (StreamWriter sw = new StreamWriter("arcadehighscores.csv", false))
                {
                    foreach (var (name, score) in highScoresList)
                    {
                        sw.WriteLine($"{name},{score}");    //write back to csv file
                    }
                }
            }
            Console.WriteLine("Enter anything to continue");
            Console.ReadKey();
        }
    }
}