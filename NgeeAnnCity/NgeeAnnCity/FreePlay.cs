using System.ComponentModel;
﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace NgeeAnnCity
{
    class FreePlayGame
    {
        private int size = 5;
        private Board board;
        private int coins;
        private int points;
        private int profit;
        private int upkeep;
        private int turn;

        private int expansionCount = 0;

        public FreePlayGame()
        {
            coins = 0;
            points = 0;
            profit = 0;
            upkeep = 0;
            turn = 0;
            board = new Board(size);
        }

        public void Start()
        {
            board.Initialize();
            PlayGame();
        }

        /*public void PlayGame()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                turn++;
                board.Display();
                DisplayInfo();

            if (row >= 0 && row < InitialGridSize && col >= 0 && col < InitialGridSize && grid[row, col] == '.')
            {
                grid[row, col] = building;
                if (row == 0 || row == (4 + 10 * expansionCount) || col == 0 || col== 4 + 10 * expansionCount) //Check for building placement on border
                {
                    expansionCount++;
                    ExpandMap(expansionCount);
                }
            }
            else
            {
                Console.WriteLine("Please input an integer for the grid's row and column");
                Console.ReadKey();
                char building = GetUserBuilding();
                board.PlaceBuilding(building);
            }
        }*/

        private void UpdateScoresandFinances()
        {
            score = 0;
            profit = 0;
            upkeep = 0;
            bool[,] visited = new bool[InitialGridSize, InitialGridSize];
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
            queue.Enqueue((row, col));  //enqueues the 'R' cell coords to check for cluster

            while (queue.Count > 0)
            {
                var (currentRow, currentCol) = queue.Dequeue(); //get first cell coords from queue
                if (currentRow >= 0 && currentRow < InitialGridSize && currentCol >= 0 && currentCol < InitialGridSize
                    && grid[currentRow, currentCol] == building && !visited[currentRow, currentCol]) /*checks if grid[currentRow, currentCol] is 'R' and is a cell that has NOT been visited,
                                                                                                      code exits out of loop if particular cell is not 'R'*/
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
        

        private void ExpandMap(int expansionCount)
        {
            InitializeGrid(InitialGridSize + (10 * expansionCount)); /* doesnt this reset the board without saving the building placed*/
        }
    }
}