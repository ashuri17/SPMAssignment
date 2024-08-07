﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;

namespace NgeeAnnCity
{
    public class FreePlayGame
    {
        private Board board;
        private int points;
        private int profit;
        private int upkeep;
        private int turn;
        private int endGameTurns;
        private bool breakCond = false;


        public Board Board { get { return this.board; } set { this.board = value; } }
        public int Profit { get { return this.profit; } set { this.profit = value; } }
        public int Turn { get { return this.turn; } set { this.turn = value; } }
        public int Points { get { return this.points; } set { this.points = value; } }
        public int Upkeep { get { return this.upkeep; } set { this.upkeep = value; } }
        public int EndGameTurns { get { return this.endGameTurns; } set { this.endGameTurns = value; } }




        public FreePlayGame()
        {
            points = 0;
            profit = 0;
            upkeep = 0;
            turn = 1;
            endGameTurns = 0;
            board = new Board(5, 25);
        }

        public void Start()
        {
            PlayGame();
        }

        public void PlayGame()
        {
            board.Initialize();
            board.InitBuilding();
            while (true)
            {
                if (board.GetSize() > board.GetMaxScreenSize())
                {
                    HandleActionWithPan();
                } 
                else
                {
                    HandleAction();
                }
                turn++;
                UpdateScoresandFinances();
                if (breakCond)
                {
                    Console.Clear();
                    break;
                }
                //Game ends when profit < upkeep for 20 turns
                if (EndGame())
                {
                    endGameTurns++;
                    if (endGameTurns == 10)
                    {
                        DisplayEndGame();
                        break;
                    }
                }
                else 
                {
                    // resets end game turns back to 0 when profit is greater than upkeep during one turn
                    endGameTurns = 0; 
                }  
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
                                    upkeep += 1;
                                    if (board.CountAdjacent(i, j, 'R') >= 1)  //checks for 'R' cluster exists
                                    {
                                        MarkCluster(i, j, 'R', visited);    //enters MarkCluster with the particular info
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
            if (board.IsAdjacentTo(row, col, 'I'))
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
                    Console.Write("Invalid choice. ");
                    continue;
                }
                return char.Parse(choice.ToUpper());
            }
        }
        private bool EndGame()
        {
            return profit < upkeep;
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
            List<(string name, int score)> highScoresList = new();

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
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            board.DisplayBoard();
            DisplayInfo();
        }
        private void HandleAction()
        {
            int choice;

            while (true)
            {
                DisplayScreen();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[1] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Construct, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[2] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Demolish, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[3] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Save: ");

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
                    FreePlaySaveFile newSave = new FreePlaySaveFile(this, "Freeplay");
                    newSave.CreateJsonFile();
                    breakCond = true;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[INFO]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Sucessfully saved file.\nPress any key to continue...");
                    Console.ReadKey();
                }
                break;
            }
        }
        private void HandleActionWithPan()
        {
            char? action;
            bool end = false;

            while (!end)
            {
                DisplayScreen();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[1] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Construct, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[2] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Demolish, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[3] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Save");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[w] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("UP, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[a] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("LEFT, ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[s] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("DOWN ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[d] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("RIGHT");
                
                action = Console.ReadKey().KeyChar;
                switch (action)
                {
                    case '1':
                        Console.WriteLine();
                        ConstructBuilding();
                        end = true;
                        break;
                    case '2':
                        Console.WriteLine();
                        if (DemolishBuilding()) {
                            end= true;
                        }
                        break;
                    case '3':
                        Console.WriteLine();
                        FreePlaySaveFile newSave = new FreePlaySaveFile(this, "Freeplay");
                        newSave.CreateJsonFile();
                        breakCond = true;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[INFO] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Sucessfully saved file.\nPress any key to continue...");
                        Console.ReadKey();
                        end = true;
                        break;
                    case 'w':
                        board.PanUp();
                        break;
                    case 'a':
                        board.PanLeft();
                        break;
                    case 's':
                        board.PanDown();
                        break;
                    case 'd':
                        board.PanRight();
                        break;
                    default:
                        break;
                }
            }
        }
        private void ConstructBuilding()
        {
            char building = GetUserBuilding();
            PlaceBuilding(building);
        }
        private Boolean DemolishBuilding()
        {
            if (board.isGridEmpty())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.White;
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
                    Console.Write($"Row (");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("1");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" - ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{size}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("): ");

                    // check if user enters a number that falls within the width of the board
                    if (!int.TryParse(Console.ReadLine(), out x) || x < 1 || x > size)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"[ERROR] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Invalid row.\n");
                        continue;
                    }
                    break;
                }

                // get column from user 
                while (true)
                {
                    Console.Write($"Column (");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("1");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" - ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{size}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("): ");

                    // check if user enters a number that falls within the height of the board
                    if (!int.TryParse(Console.ReadLine(), out y) || y < 1 || y > size)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"[ERROR] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Invalid column.\n");
                        continue;
                    }
                    break;
                }

                x--;
                y--;

                // check if spot is taken
                if (board.GetBuilding(x, y) != '.')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"[ERROR] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Spot taken.\n");
                    continue;
                }
                else
                {
                    board.PlaceBuilding(building, x, y);
                    if (board.TouchingBorder(x, y))
                    {
                        board.ExpandGrid();
                        board.PanTo(x + board.GetExpansionSize(), y + board.GetExpansionSize());
                    } 
                    else 
                    {
                        board.PanTo(x, y);
                    }
                }
                break;
            }
        }
    }
}