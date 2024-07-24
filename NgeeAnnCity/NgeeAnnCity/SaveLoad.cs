using System.Dynamic;
using System.IO;
using System.Text.Json;
using Microsoft.VisualBasic;
using NgeeAnnCity;

class SaveFile
{
    public DateTime SaveDateTime = DateTime.Now;
    public int SaveID = Directory.GetFiles("saves").Count() + 1;
    public string? SaveDesc {get; set;}
}

class ArcadeSaveFile:SaveFile
{
    public Arcade GameData {get; set;}

    public ArcadeSaveFile(Arcade gD, string sDesc)
    {
        GameData = gD;
        SaveDesc = sDesc;
    }

    //Takes Arcade class object, appends additional required data to the end and converts values to a json string to be stored in a file
    //To create a save file: ArcadeSaveFIle saveFile = new ArcadeSaveFile(arcadeObj, SaveDesc)
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
    private static Arcade LoadFile()
    {
        // directory path for save files
        string saveFilesDirectory = "saves";

        // get save files in directory
        string[] saveFiles = Directory.GetFiles(saveFilesDirectory);

        // check if directory is empty
        if (!saveFiles.Any())
        {
            Console.WriteLine("No save file available");
            return null;
        }

        // print save files for user to choose
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"[{i}] {saveFiles[i]}");
        }


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
            string saveFileGameData;
            using (StreamReader sr = new StreamReader(saveFiles[result]))
            {
                saveFileGameData = sr.ReadLine();
            }

            Arcade loadedGameFile = JsonSerializer.Deserialize<Arcade>(saveFileGameData);
            return loadedGameFile;
        }                                                                       
  

    }
}

class FreePlaySaveFile:SaveFile
{
    public FreePlayGame GameData {get; set;}

    public void CreateJsonFile()
    {
        var gameDataJson = JsonSerializer.Serialize(GameData);
        var timeStampJson = JsonSerializer.Serialize(SaveDateTime);
        var saveDescJson = JsonSerializer.Serialize(SaveDesc);
        var dataJson = new List<string>{gameDataJson, timeStampJson, saveDescJson};
        using (StreamWriter sw = new StreamWriter($"save{SaveID}.json", false))
        {
            foreach (string s in dataJson)
            {
                sw.WriteLine(s);
                Console.WriteLine(s);
            }
        }
            
    }
    private static FreePlayGame LoadFile()
    {
        // directory path for save files
        string saveFilesDirectory = "saves";

        // get save files in directory
        string[] saveFiles = Directory.GetFiles(saveFilesDirectory);

        // check if directory is empty
        if (!saveFiles.Any())
        {
            Console.WriteLine("No save file available");
            return null;
        }

        // print save files for user to choose
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"[{i}] {saveFiles[i]}");
        }


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
            string saveFileGameData;
            using (StreamReader sr = new StreamReader(saveFiles[result]))
            {
                saveFileGameData = sr.ReadLine();
            }

            FreePlayGame loadedGameFile = JsonSerializer.Deserialize<FreePlayGame>(saveFileGameData);
            return loadedGameFile;
        }                                                                       
  

    }
}