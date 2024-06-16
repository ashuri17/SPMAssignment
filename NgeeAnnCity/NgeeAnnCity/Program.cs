using NgeeAnnCity;
static void MainMenu()
{
    // title of App
    string title = "NGEE ANN CITY";

    // add items into menu
    List<string> menuItems = [];
    menuItems.Add("1. New Arcade Game");
    menuItems.Add("2. New Free Play Game");
    menuItems.Add("3. Load Saved Game");
    menuItems.Add("4. Display Highscores");
    menuItems.Add("5. Exit App");

    // get padding to center title when printing
    string padding = new string(' ', (int)Math.Floor((double)(menuItems.Max(i => i.Length) - title.Length) / 2));

    // print title + menu
    Console.WriteLine(padding + title);
    Console.WriteLine(new string('-', menuItems.Max(i => i.Length)) + "\n");
    menuItems.ForEach(i => Console.WriteLine(i));
}

while (true)
{
    MainMenu();
    Console.Write("Pick option (1-5): ");
    string userInput = Console.ReadLine();

    if (int.TryParse(userInput, out int option))
    {
        if (option == 1)
        {
            Arcade arcadeGame = new Arcade();
            arcadeGame.Start();
        }
        else if (option == 2)
        {
            StartNewFreePlayGame();
        }
        else if (option == 3)
        {

        }
        else if (option == 4)
        {
            HighScores highScores = new HighScores();
            highScores.Start();
        }
        else if (option == 5)
        {
            Console.WriteLine("Exiting the game. Goodbye!");
            break;
        }
        else
        {
            Console.WriteLine("Invalid option. Please pick a number between 1 and 5.");
        }
    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a number.");
    }
}
void StartNewFreePlayGame()
{
    FreePlayGame game = new FreePlayGame();
    game.Play();
}