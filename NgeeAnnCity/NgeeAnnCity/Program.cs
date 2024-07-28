using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using NgeeAnnCity;



StartNgeeAnnCity();
void StartNgeeAnnCity()
{
    // Does nothing if dir exists
    Directory.CreateDirectory("ArcadeSave");
    Directory.CreateDirectory("FreePlaySave");

    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Black;
    Console.Clear();

    while (true)
    {
        MainMenu();
        Console.Write("\n\n\nPick option (1-5): ");
        char option = Console.ReadKey().KeyChar;
        Console.WriteLine();
        switch (option)
        {
            case '1':
                new Arcade().Start();
                ClearScreen();
                break;

            case '2':
                new FreePlayGame().Start();
                ClearScreen();
                break;

            case '3':
                bool end = false;
                while (!end)
                {
                    ClearScreen();
                    Console.Write("\nFreeplay ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[f]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("/ Arcade ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[a]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");

                    char choice = Console.ReadKey().KeyChar;
                    Console.WriteLine("\n\n");

                    switch (choice)
                    {
                        case 'a':
                            end = true;

                            if (Directory.GetFiles("ArcadeSave").Length == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("[INFO] ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("No Arcade save file.\n");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            } 
                            else
                            {
                                ClearScreen();
                                SaveFile.LoadScreen(true);
                            }
                            
                            break;

                        case 'f':
                            end = true;

                            if (Directory.GetFiles("FreePlaySave").Length == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("[INFO] ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("No Freeplay save file.\n");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                ClearScreen();
                                SaveFile.LoadScreen(false);
                            }
                            break;

                        default: 
                            Console.Write("Invalid Option. ");
                            break;
                    }
                }
                ClearScreen();
                break;

            case '4':
                ClearScreen();
                HighScores.Start();
                ClearScreen();
                break;

            case '5':
                Console.WriteLine("Exiting Ngee Ann City...");
                Thread.Sleep(700);
                Console.WriteLine("Goodbye!\n");
                return;

            default:
                // User entered a number that is not valid as an option
                ClearScreen();
                break;
        }
    }
}

void ClearScreen()
{
    Console.Clear();
    Console.WriteLine("\x1b[3J");
}

void MainMenu()
{
    // title of App
    string title = "NGEE ANN CITY";
 
    // add items into menu
    List<string> menuItems = [];
    menuItems.Add("New Arcade Game");
    menuItems.Add("New Free Play Game");
    menuItems.Add("Load Saved Game");
    menuItems.Add("Display Highscores");
    menuItems.Add("Exit App");
 
    // get padding to center title when printing
    string padding = new string(' ', (int)Math.Floor((double)(menuItems.Max(i => i.Length) - title.Length) / 2));
 
    // print title + menu
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(padding + title);
    Console.WriteLine(new string('-', menuItems.Max(i => i.Length)) + "\n");
    
    for (int i = 1; i <= menuItems.Count; i++)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[{i}] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(menuItems[i - 1]);
    }
}
