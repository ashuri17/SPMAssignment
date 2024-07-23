using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        private int firstRow = 0;
        private int firstCol = 0;
        private int maxScreenSize = 25;

        public FreePlayGame()
        {
            coins = 0;
            points = 0;
            profit = 0;
            upkeep = 0;
            turn = 1;
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
                DisplayScreen();
                if (board.GetSize() > maxScreenSize)
                {
                    while (true)
                    {
                        Console.WriteLine("Pan Map (1) or Place Building (2): ");
                        string? choice = Console.ReadLine();

                        if (string.IsNullOrEmpty(choice) || !"12".Contains(choice.ToUpper()) || choice.Length != 1)
                        {
                            Console.WriteLine("Invalid choice, try again.");
                            continue;
                        }

                        if (choice == "2")
                        {
                            break;
                        }

                        PanBoard();
                        DisplayScreen();
                    }
                }
                int buildingAction = board.ConstructOrDemolish();
                if (buildingAction == 1)    //construct option
                {
                    char building = GetUserBuilding();
                    board.PlaceBuilding(building, turn, true);
                }
                else if (buildingAction == 2)   //demolish option
                {
                    if (board.isGridEmpty())
                    {
                        Console.WriteLine("Board consists of no buildings.");
                        Console.WriteLine("Press any key to return...");
                        Console.ReadKey();
                        continue;
                    }
                    board.DemolishBuilding();
                }
                turn++;
                UpdateScoresandFinances();
                //Game ends when profit < upkeep for 20 turns
                if (EndGame())
                {
                    endGameTurns++;
                    if (endGameTurns == 2)
                    {
                        DisplayEndGame();
                        break;
                    }
                }
                else { endGameTurns = 0; }  // resets end game turns back to 0 when profit is greater than upkeep during one turn
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
                                    if (board.CountAdjacent(i, j, 'R') >= 1)  //checks for 'R' cluster exists
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
                                points += 1;
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
                                points += CalculateRoadScore(i, j);
                                if (!board.IsAdjacentTo(i, j, 'R') && !board.IsAdjacentTo(i, j, 'I') &&
                                    !board.IsAdjacentTo(i, j, 'C') && !board.IsAdjacentTo(i, j, 'O') &&
                                    !board.IsAdjacentTo(i, j, '*'))   //checks if road is NOT adjacent to any other buildings
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
            if (board.FreePlayIsAdjacentTo(row, col, 'I'))
            {
                points += 1;
                return points;
            }
            points += board.CountAdjacentFreePlay(row, col, 'R') + board.CountAdjacentFreePlay(row, col, 'C') + 2 * board.CountAdjacentFreePlay(row, col, 'O');
            return points;
        }


        private int CalculateCommercialScore(int row, int col)
        {
            return board.CountAdjacentFreePlay(row, col, 'C');
        }

        private int CalculateParkScore(int row, int col)
        {
            return board.CountAdjacentFreePlay(row, col, 'O');
        }

        private int CalculateRoadScore(int row, int col)
        {
            return board.CountAdjacentRow(row, col, '*');
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
                    queue.Enqueue((currentRow - 1, currentCol));//left
                    queue.Enqueue((currentRow + 1, currentCol));//right
                    queue.Enqueue((currentRow, currentCol - 1));//down
                    queue.Enqueue((currentRow, currentCol + 1));//up

                    queue.Enqueue((currentRow - 1, currentCol - 1)); // down-left
                    queue.Enqueue((currentRow + 1, currentCol + 1)); // up-right
                    queue.Enqueue((currentRow + 1, currentCol - 1)); // down-right
                    queue.Enqueue((currentRow - 1, currentCol + 1)); // up-left

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

            string[] freeplayHighScores = File.ReadAllLines("./freeplayhighscores.csv").Skip(1).ToArray();

            foreach (string line in freeplayHighScores)
            {
                string[] parts = line.Split(',');   //splits the playerName and points
                highScoresList.Add((parts[0], int.Parse(parts[1])));
            }
            HighScores.ViewFreePlay(); //display freeplay leaderboard
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
                    highScoresList.RemoveAt(10);    //ensures list only contains 10 entries
                }
                using (StreamWriter sw = new StreamWriter("freeplayhighscores.csv", false))
                {
                    sw.WriteLine("Name,Points");    //header in csv
                    foreach (var (name, score) in highScoresList)
                    {
                        sw.WriteLine($"{name},{score}");    //write back to csv file
                    }
                }
            }
            Console.WriteLine("Enter anything to continue");
            Console.ReadKey();
        }

        private void DisplayScreen()
        {
            //Console.Clear();
            Console.WriteLine("\x1b[3J");
            if (board.GetSize() < maxScreenSize)
            {
                board.Display(firstRow, firstCol, board.GetSize());
            }
            else
            {
                board.Display(firstRow, firstCol, maxScreenSize);

            }
            DisplayInfo();
        }

        private void PanBoard()
        {
            char? direction;
            bool end = false;

            while (!end)
            {
                DisplayScreen();
                Console.WriteLine("w - Up, a - Left, s - Down, d - Right, q - Quit");

                direction = Console.ReadKey().KeyChar;
                switch (direction)
                {
                    case 'w':
                        firstRow = firstRow - maxScreenSize < 0 ? 0 : firstRow - maxScreenSize; // 0 = 0 - 25 < 0 ? 0 : 0 - 25
                        break;
                    case 'a':
                        firstCol = firstCol - maxScreenSize < 0 ? 0 : firstCol - maxScreenSize;
                        break;
                    case 's':
                        firstRow = firstRow + maxScreenSize > board.GetSize() - maxScreenSize ? board.GetSize() - maxScreenSize : firstRow + maxScreenSize; // 0 = 0 + 25 > 15 ? 15 : 25 => 15
                        break;
                    case 'd':
                        firstCol = firstCol + maxScreenSize > board.GetSize() - maxScreenSize ? board.GetSize() - maxScreenSize : firstCol + maxScreenSize;
                        break;
                    case 'q':
                        end = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}