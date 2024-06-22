using NgeeAnnCity;
//hello
int choice;
do
{
    DisplayMenu();
    choice = GetChoice();

    try
    {
        // Enter user input
        if (choice == 1)
        {
            //StartNewArcadeGame();
        }
        else if (choice == 2)
        {
            StartNewFreePlayGame();
        }
        else if (choice == 3)
        {
            LoadSavedGame();
        }
        else if (choice == 4)
        {
            DisplayHighScores();
        }
        else if (choice == 0)
        {
            Console.WriteLine("Bye!");
        }
        else
        {
            Console.WriteLine("Invalid option! Please try again.");
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine("Error: " + ex.Message);
        Console.WriteLine("Invalid input. Please enter a valid number.");
    }

    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }

} while (choice != 0);
//To display the menu
void DisplayMenu()
{
    Console.WriteLine("Welcome to Ngee Ann City Game");
    Console.WriteLine("[1] Start new arcade game");
    Console.WriteLine("[2] Start new free play game");
    Console.WriteLine("[3] Load saved game");
    Console.WriteLine("[4] Display high scores");
    Console.WriteLine("[0] Exit");
    Console.WriteLine("---------------------------------");
}

int GetChoice()
{
    int choice;
    Console.Write("Enter your option: ");

    try
    {
        choice = Convert.ToInt32(Console.ReadLine());
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid input. Please enter a valid number.");
        choice = GetChoice();
    }
    return choice;
}


void StartNewFreePlayGame()
{
    FreePlayGame game = new FreePlayGame();
    game.Play();
}

void LoadSavedGame()
{
    // Implement load game functionality
}

void DisplayHighScores()
{
    // Implement high score display functionality
}