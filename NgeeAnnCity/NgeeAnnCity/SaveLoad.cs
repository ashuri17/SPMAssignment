using System.Dynamic;
using System.IO;
using System.Text.Json;
using Microsoft.VisualBasic;
using NgeeAnnCity;

public class SaveFile
{
    public DateTime SaveDateTime = DateTime.Now;
    public int SaveID = Directory.GetFiles("saves").Count() + 1;
    public string? SaveDesc {get; set;}

    public static void LoadScreen()
    {
        // directory path for save files
        string saveFilesDirectory = "saves";

        // get save files in directory
        string[] saveFiles = Directory.GetFiles(saveFilesDirectory);

        // check if directory is empty
        if (!saveFiles.Any())
        {
            Console.WriteLine("No save file available");
        }

        // print save files for user to choose
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"[{i}] {saveFiles[i]}");
        }
        GamemodeChecker(saveFiles);
    }
    public static void GamemodeChecker(string[] saveFiles)
    {
        while (true)
        {
            Console.WriteLine("\nEnter the number corresponding to the saved game: ");

            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid option");
                continue;
            }

            if (result < 0 || result > saveFiles.Length - 1)
            {
                Console.WriteLine("That save file does not exist.");
                continue;
            }
            List<string> saveFileGameData = new();
            using (StreamReader sr = new StreamReader(saveFiles[result]))
            {
                for (int i = 0; i < 3; i++)
                {
                    saveFileGameData.Add(sr.ReadLine());
                }
            }
            if (JsonSerializer.Deserialize<string>(saveFileGameData[2]) == "Arcade")
            {
                LoadArcade(saveFileGameData[0]).PlayGame();
                break;
            }
            else if (JsonSerializer.Deserialize<string>(saveFileGameData[2]) == "FreePlay")
            {
                LoadFreePlay(saveFileGameData[0]).PlayGame();
                break;
            }
            else
            {
                Console.WriteLine("An error has occured");
            }
        }
    }

    public static Arcade LoadArcade(string saveFile)
    {
        Arcade loadedGameFile = JsonSerializer.Deserialize<Arcade>(saveFile);
        return loadedGameFile;                                                                            
    }

    public static FreePlayGame LoadFreePlay(string saveFile)
    {
        FreePlayGame loadedGameFile = JsonSerializer.Deserialize<FreePlayGame>(saveFile);
        return loadedGameFile;
    }
}

public class ArcadeSaveFile:SaveFile
{
    public Arcade GameData {get; set;}

    public ArcadeSaveFile(Arcade gD, string sDesc)
    {
        GameData = gD;
        SaveDesc = sDesc;
    }

    //Takes Arcade class object, appends additional required data to the end and converts values to a json to be stored in a file
    public void CreateJsonFile()
    {
        var gameDataJson = JsonSerializer.Serialize(GameData);
        var timeStampJson = JsonSerializer.Serialize(SaveDateTime);
        var saveDescJson = JsonSerializer.Serialize(SaveDesc);
        var dataJson = new List<string>{gameDataJson, timeStampJson, saveDescJson};
        using (StreamWriter sw = new StreamWriter($"saves\\save{SaveID}.json", false))  
        {
            foreach (string s in dataJson)
            {
                sw.WriteLine(s);
                Console.WriteLine(s);
            }
        } 
    }
}

public class FreePlaySaveFile:SaveFile
{
    public FreePlayGame GameData {get; set;}

    public FreePlaySaveFile(FreePlayGame gD, string sDesc)
    {
        GameData = gD;
        SaveDesc = sDesc;
    }

    public void CreateJsonFile()
    {
        var gameDataJson = JsonSerializer.Serialize(GameData);
        var timeStampJson = JsonSerializer.Serialize(SaveDateTime);
        var saveDescJson = JsonSerializer.Serialize(SaveDesc);
        var dataJson = new List<string>{gameDataJson, timeStampJson, saveDescJson};
        using (StreamWriter sw = new StreamWriter($"saves\\save{SaveID}.json", false))  
        {
            foreach (string s in dataJson)
            {
                sw.WriteLine(s);
                Console.WriteLine(s);
            }
        } 
    }
}