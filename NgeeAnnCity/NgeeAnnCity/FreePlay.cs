using System.ComponentModel;
﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace NgeeAnnCity
{
    class FreePlayGame
    {
        private Board board;
        private int coins;
        private int points;
        private int profit;
        private int upkeep;
        private int turn;
        private int endGameTurns;

        private int expansionCount = 0;

        public FreePlayGame()
        {
            coins = 0;
            points = 0;
            profit = 0;
            upkeep = 0;
            turn = 0;
            endGameTurns = 0;
            board = new Board(5);
        }

        public void Start()
        {
            board.Initialize();
            PlayGame();
        }

        public void PlayGame()
        {
            while (true)
            {
                //Console.Clear();
                //Console.WriteLine("\x1b[3J");
                turn++;
                board.Display();
                DisplayInfo();
                char building = GetUserBuilding();
                board.PlaceBuilding(building, true);
                UpdateScoresandFinances();
                if (EndGame())
                {
                    endGameTurns++;
                    if (endGameTurns == 5)
                    {
                        DisplayEndGame();
                        break;
                    }
                }
                else { endGameTurns = 0; }  // resets end game turns back to 0 when profit is greater than upkeep
            }
        }

        private void UpdateScoresandFinances()
        {
            points = 0;
            profit = 0;
            upkeep = 0;
            int size = board.GetSize();
            bool[,] visited = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    char building = board.GetBuilding(i, j);
                    if (building != '.')
                    {
                        switch (building)
                        {
                            case 'R':
                                points += CalculateResidentialScore(i, j);
                                profit += 1;
                                if (!visited[i, j])
                                {
                                    if (CountAdjacent(i, j, 'R') >= 1)  //checks for 'R' cluster exists
                                    {
                                        MarkCluster(i, j, 'R', visited);    //enters MarkCluster with the particular info
                                        upkeep += 1;
                                    }
                                    else
                                    {
                                        upkeep += 1;
                                    }
                                }
                                break;
                            case 'I':
                                points += CalculateIndustryScore();
                                profit += 2;
                                upkeep += 1;
                                break;
                            case 'C':
                                points += CalculateCommercialScore(i, j);
                                profit += 3;
                                upkeep += 2;
                                break;
                            case 'O':
                                points += CalculateParkScore(i, j);
                                upkeep += 1;
                                break;
                            case '*':
                                points += CalculateRoadScore(i);
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
            int points = 0;
            if (IsAdjacentTo(row, col, 'I'))
            {
                return 1;
            }
            points += CountAdjacent(row, col, 'R') + CountAdjacent(row, col, 'C') + 2 * CountAdjacent(row, col, 'O');
            return points;
        }

        private int CalculateIndustryScore()
        {
            int industryCount = 0;
            int size = board.GetSize();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
            int roadScore = 0;

            for (int col = 0; col < board.GetSize(); col++)
            {
                if (board.GetBuilding(row, col) == '*')
                {
                    roadScore++;
                }
            }
            return roadScore;
        }

        private bool IsAdjacentTo(int row, int col, char building)
        {
            int size = board.GetSize();

            return (row > 0 && board.GetBuilding(row - 1, col) == building) ||
                   (row < size - 1 && board.GetBuilding(row + 1, col) == building) ||
                   (col > 0 && board.GetBuilding(row, col - 1) == building) ||
                   (col < size - 1 && board.GetBuilding(row, col + 1) == building);
        }

        private int CountAdjacent(int row, int col, char building)
        {
            int count = 0;
            int size = board.GetSize();
            if (row > 0 && board.GetBuilding(row - 1, col) == building) count++;
            if (row < size - 1 && board.GetBuilding(row + 1, col) == building) count++;
            if (col > 0 && board.GetBuilding(row, col - 1) == building) count++;
            if (col < size - 1 && board.GetBuilding(row, col + 1) == building) count++;
            return count;
        }
        private void DisplayInfo() 
        {
            Console.WriteLine(new string('-', 10) + "FREEPLAY MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins: {coins}");
            Console.WriteLine($"Points: {points}");
            Console.WriteLine($"Profit: {profit}");
            Console.WriteLine($"Upkeep: {upkeep}\n");
        }

        private void MarkCluster(int row, int col, char building, bool[,] visited)
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();
            int size = board.GetSize();
            queue.Enqueue((row, col));  //enqueues the 'R' cell coords to check for cluster

            while (queue.Count > 0)
            {
                var (currentRow, currentCol) = queue.Dequeue(); //get first cell coords from queue

                /*checks if grid[currentRow, currentCol] is 'R' and is a cell that has NOT been visited, code exits out of loop if particular cell is not 'R'*/
                if (currentRow >= 0 && currentRow < size && currentCol >= 0 && currentCol < size
                    && board.GetBuilding(currentRow, currentCol) == building && !visited[currentRow, currentCol]) 
                {
                    visited[currentRow, currentCol] = true; //sets particular cell as a 'visited' cell to prevent UpdateScoresandFinances() from tracking this cell

                    // enqueues the 4 adjacent cell coords for the next loop
                    queue.Enqueue((currentRow - 1, currentCol));
                    queue.Enqueue((currentRow + 1, currentCol));
                    queue.Enqueue((currentRow, currentCol - 1));
                    queue.Enqueue((currentRow, currentCol + 1));
                }
            }
        }

        private char GetUserBuilding()
        {
            while (true)
            {
                Console.Write("Choose a building to construct (R, I, C, O, *): ");
                string? choice = Console.ReadLine();

                if (string.IsNullOrEmpty(choice) || !"RICO*".Contains(choice.ToUpper()) || choice.Length != 1)
                {
                    Console.WriteLine("Invalid choice, try again.");
                    continue;
                }
                return char.Parse(choice.ToUpper());
            }
        }
        private bool EndGame()
        {
            if (profit < upkeep)
            {
                return true;
            }
            return false;
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

            string[] freeplayHighScores = HighScores.ViewFreePlay();

            foreach (string line in freeplayHighScores)
            {
                string[] parts = line.Split(',');   //splits the playerName and points
                highScoresList.Add((parts[0], int.Parse(parts[1])));
            }
            if (points > highScoresList[^1].score)
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
                    highScoresList.RemoveAt(10);    //ensurs list only contains 10 entries
                }
                using (StreamWriter sw = new StreamWriter("freeplayhighscores.csv", false))
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