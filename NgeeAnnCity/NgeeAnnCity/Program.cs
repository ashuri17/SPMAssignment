using System.Text.Json;
using System.Text.Json.Serialization;
using NgeeAnnCity;



StartNgeeAnnCity();
void StartNgeeAnnCity()
{
    while (true)
    {
        MainMenu();
        Console.Write("\n\n\nPick option (1-5): ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.ForegroundColor = ConsoleColor.Green;
                new Arcade().Start();
                break;

            case "2":
                Console.ForegroundColor = ConsoleColor.Yellow;
                new FreePlayGame().Start();
                break;

            case "3":
                Console.ForegroundColor = ConsoleColor.Blue;
                SaveFile.LoadScreen();
                Console.WriteLine();
                break;

            case "4":
                Console.ForegroundColor = ConsoleColor.Magenta;
                HighScores.Start();
                break;

            case "5":
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exiting Ngee Ann City...");
                Thread.Sleep(700);
                Console.WriteLine("Goodbye!\n");
                return;

            default:
                // User entered a number that is not valid as an option
                Console.WriteLine("\nInvalid option");
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
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(padding + title);
    Console.WriteLine(new string('-', menuItems.Max(i => i.Length)) + "\n");
    Console.ResetColor();
    menuItems.ForEach(i =>
    {
        SetMenuItemColor(i);
        Console.WriteLine(i);
        Console.ResetColor();
    });
}

void SetMenuItemColor(string menuItem)
{
    if (menuItem.Contains("Arcade"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }
    else if (menuItem.Contains("Free Play"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else if (menuItem.Contains("Saved Game"))
    {
        Console.ForegroundColor = ConsoleColor.Blue;
    }
    else if (menuItem.Contains("Highscores"))
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
    }
    else if (menuItem.Contains("Exit"))
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
}