using System.ComponentModel.DataAnnotations;
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
                Console.WriteLine("Invalid location or cell already occupied. Try again.");
                Console.ReadKey();
                char building = GetUserBuilding();
                board.PlaceBuilding(building);
            }
        }*/

        private void DisplayInfo()
        {
            Console.WriteLine(new string('-', 10) + "FREEPLAY MODE" + new string('-', 10) + "\n");
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins: {coins}");
            Console.WriteLine($"Points: {points}");
            Console.WriteLine($"Profit: {profit}");
            Console.WriteLine($"Upkeep: {upkeep}\n");
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
        private void DisplayInfo(int turn)
        {
            Console.WriteLine($"Turn: {turn}");
            Console.WriteLine($"Coins: {coins}");
            Console.WriteLine($"Score: {score}");
            Console.WriteLine($"Profit: {profit}");
            Console.WriteLine($"Upkeep: {upkeep}");
        }

        private void ExpandMap(int expansionCount)
        {
            InitializeGrid(InitialGridSize + (10 * expansionCount)); /* doesnt this reset the board without saving the building placed*/
        }
    }
}