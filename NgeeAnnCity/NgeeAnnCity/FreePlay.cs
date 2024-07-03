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
        private int firstRow = 1;
        private int firstCol = 1;
        private int screenSize = 25;

        public FreePlayGame()
        {
            coins = 0;
            points = 0;
            profit = 0;
            upkeep = 0;
            turn = 0;
            board = new Board(50);
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
                turn++;
                DisplayScreen();
                if (board.GetSize() > screenSize)
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
                char building = GetUserBuilding();
                board.PlaceBuilding(building, true);
                UpdateScoresandFinances();
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

        private void DisplayScreen()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            board.Display();
            DisplayInfo();
        }

        private void PanBoard()
        {
            char? direction;
            bool end = false;

            while (!end)
            {
                DisplayScreen();
                Console.WriteLine(firstRow);
                Console.WriteLine(firstCol);
                Console.WriteLine("w - Up, a - Left, s - Down, d - Right, q - Quit");

                direction = Console.ReadKey().KeyChar;
                switch (direction)
                {
                    case 'w':
                        firstCol = firstCol - screenSize < 1 ? 1 : firstCol - screenSize;
                        break;
                    case 'a':
                        firstRow = firstRow - screenSize < 1 ? 1 : firstRow - screenSize;
                        break;
                    case 's':
                        firstCol = firstCol + screenSize > board.GetSize() - screenSize ? board.GetSize() - screenSize : firstCol + screenSize;
                        break;
                    case 'd':
                        firstRow = firstRow + screenSize > board.GetSize() - screenSize ? board.GetSize() - screenSize : firstRow + screenSize;
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