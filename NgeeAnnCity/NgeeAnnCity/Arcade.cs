using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;

namespace NgeeAnnCity
{
    public class Arcade
    {
        private Board board;
        private int profit;
        private int turn;
        private int coins;
        private int points;
        private string[] buildings;

        public Board Board { get { return this.board; } set { this.board = value; } }
        public int Profit { get { return this.profit; } set { this.profit = value; } }
        public int Turn { get { return this.turn; } set { this.turn = value; } }
        public int Coins { get { return this.coins; } set { this.turn = value; } }
        public int Points { get { return this.points; } set { this.points = value; } }
        public string[] Buildings { get { return this.buildings; } set { this.buildings = value; } }

        public Arcade()
        {
            board = new Board(20, 20);
            turn = 1;
            coins = 16;
            points = 0;
            buildings = new string[] { "Residential", "Industry", "Commercial", "Park", "Road" };
        }

        public void Start()
        {
            board.Initialize();
            PlayGame();
        }

        public void PlayGame()
        {
            while (coins > 0 && !board.isGridFull())   //end game conditions
            {
                HandleAction();
                coins -= 1;
                UpdateScoresAndFinances();
                turn++;
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
                string building = buildings[new Random().Next(buildings.Length)];
                if (!selectedBuildings.Contains(building))
                {
                    selectedBuildings.Add(building);
                }
            }
            return selectedBuildings;
        }
        private char GetPlayerChoice(List<string> selectedBuildings)
        {
            int choice;

            for (int i = 0; i < selectedBuildings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {selectedBuildings[i]}");
            }

            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > selectedBuildings.Count)
            {
                Console.Write("Invalid choice. Please select 1 or 2:");
            }
            return GetBuildingSymbol(selectedBuildings[choice - 1]);
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
        private void DisplayInfo()
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
                                points += CalculateRoadScore(i, j);
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
                return points;
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
        private int CalculateRoadScore(int row, int col)
        {
            return board.CountAdjacentRow(row, col, '*');
        }
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

            string[] arcadeHighScores = File.ReadAllLines("arcadehighscores.csv").Skip(1).ToArray();
            foreach (string line in arcadeHighScores)
            {
                string[] parts = line.Split(',');   //splits the playerName and points
                highScoresList.Add((parts[0], int.Parse(parts[1])));
            }
            HighScores.ViewArcade();    //display Arcade leaderboard
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
                //overwrite arcadehighscores.csv file
                using (StreamWriter sw = new StreamWriter("arcadehighscores.csv", false))
                {
                    sw.WriteLine("Name,Points"); //header in csv
                    foreach (var (name, score) in highScoresList)
                    {
                        sw.WriteLine($"{name},{score}");    //write back to csv file
                    }
                }
            }
            Console.WriteLine("Enter anything to continue");
            Console.ReadKey();
        }
        private void HandleAction()
        {
            int choice;

            while (true)
            {
                DisplayScreen();
                Console.WriteLine("1 - Construct, 2 - Demolish, 3 - Save");

                if (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2 && choice != 3))
                {
                    continue;
                }

                if (choice == 1)
                {
                    ConstructBuilding();
                }
                else if (choice == 2)
                {
                    if (!DemolishBuilding())
                    {
                        continue;
                    }
                } 
                else
                {
                    ArcadeSaveFile newSave = new ArcadeSaveFile(this, "Arcade");
                    newSave.CreateJsonFile();
                    Console.WriteLine("Sucessfully saved file");
                }
                break;
            }
        }
        private void DisplayScreen()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            board.DisplayBoard();
            DisplayInfo();
        }
        private void ConstructBuilding()
        {
            List<String> buildings = SelectBuildings(); // !SHOULD BE STRING ARRAY
            char building = GetPlayerChoice(buildings);
            PlaceBuilding(building);
        }
        private Boolean DemolishBuilding()
        {
            if (board.isGridEmpty())
            {
                Console.WriteLine("Board consists of no buildings.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return false;
            }
            board.DemolishBuilding();
            return true;
        }
        private void PlaceBuilding(char building)
        {
            int x, y;
            int size = board.GetSize();

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
                        Console.Write($"Invalid row. ");
                        continue;
                    }
                    break;
                }

                Console.WriteLine();

                // get column from user 
                while (true)
                {
                    Console.Write($"Column (1-{size}): ");

                    // check if user enters a number that falls within the height of the board
                    if (!int.TryParse(Console.ReadLine(), out y) || y < 1 || y > size)
                    {
                        Console.Write($"Invalid column. ");
                        continue;
                    }
                    break;
                }

                x--;
                y--;

                // check if spot is taken
                if (board.GetBuilding(x, y) != '.')
                {
                    Console.Write("Spot taken.\n");
                    continue;
                }
                //check if buildings in arcade are placed adjacent to existing buildings
                else if (!board.isGridEmpty() && !board.IsAdjacentToExistingBuilding(x, y))
                {
                    Console.Write("Building must be placed adjacent to an existing building.\n");
                    continue;
                }
                else
                {
                    board.PlaceBuilding(building, x, y);
                }
                break;
            }
        }
    }
}