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

        public void PlayGame()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                turn++;
                board.Display();
                DisplayInfo();

                char building = GetUserBuilding();
                board.PlaceBuilding(building);
            }
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
    }
}