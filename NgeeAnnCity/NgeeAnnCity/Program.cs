using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using NgeeAnnCity;



StartNgeeAnnCity();
void StartNgeeAnnCity()
{
    bool gameModePick = false; // true = arcade, false = freeplay
    while (true)
    {
        MainMenu();
        Console.Write("\n\n\nPick option (1-5): ");

        switch (Console.ReadKey().KeyChar)
        {
            case '1':
                new Arcade().Start();
                break;

            case '2':
                new FreePlayGame().Start();
                break;

            case '3':
                while (true)
                {
                    Console.Write("\nFreeplay Files or Arcade Files (F or A): ");
                    string fileLoadMode = Console.ReadLine();
                    if (fileLoadMode.ToUpper() == "A")
                    {
                        gameModePick = true;
                        SaveFile.LoadScreen(gameModePick);
                        Console.WriteLine();
                        break;

                    }
                    else if (fileLoadMode.ToUpper() == "F")
                    {
                        gameModePick= false;
                        SaveFile.LoadScreen(gameModePick);
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        Console.Write("Invalid Option. ");
                    }
                }
                break;

            case '4':
                HighScores.Start();
                break;

            case '5':
                Console.WriteLine("Exiting Ngee Ann City...");
                Thread.Sleep(700);
                Console.WriteLine("Goodbye!\n");
                return;

            default:
                // User entered a number that is not valid as an option
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                break;
        }
    }
}

void MainMenu()
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
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(padding + title);
    Console.WriteLine(new string('-', menuItems.Max(i => i.Length)) + "\n");
    Console.ForegroundColor = ConsoleColor.Green;

    menuItems.ForEach(i => Console.WriteLine(i));

}
