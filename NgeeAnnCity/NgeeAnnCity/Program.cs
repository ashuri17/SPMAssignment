
using System.Text.Json;
using System.Text.Json.Serialization;

List<SaveFile> SaveScreen = new();



/*void SavingFile()
{
    Console.WriteLine("Enter a description for this save file (optional): ");
    string? UserGivenDesc = Console.ReadLine();
    if (UserGivenDesc is null)
    {
        UserGivenDesc = "No Desc";
    }

    SaveFile SF = new SaveFile
    {
        SaveDateTime = DateTime.Now,
        SaveID = SaveScreen.Count + 1,
        SaveDesc = UserGivenDesc,
        Location = "Coords"
        Stats = "DisplayStats()",
        FilePath = $"Saves\\Save{SaveScreen.Count + 1}"
    };

    // Saving save data to a newly created json file
    string saveFileName = $"Save{SF.SaveID}.json";
    string destPath = SF.FilePath;
    string jsonString = JsonSerializer.Serialize(SF);
    File.WriteAllText($"Saves\\{saveFileName}", jsonString);

    SaveScreen.Add(SF);
}*/

SaveFile? LoadingFile()
{
    SaveScreen.ForEach(i => Console.WriteLine($"{i.GameType}{i.SaveDateTime} {i.SaveID} {i.SaveDesc}"));
    Console.WriteLine("Choose a file to load:"); //User selects file to load
    string? fileLoad = Console.ReadLine();
    string path = $"Saves\\Save{fileLoad}.json";
    if (File.Exists(path)) //Checking for file
    {
        using (StreamReader r = new StreamReader(path)) //Reading file
        {
            string jsonString = r.ReadToEnd();
            SaveFile? saveFile = JsonSerializer.Deserialize<SaveFile>(jsonString);
            Console.WriteLine(saveFile.ToString());
            return saveFile;
        }
    }
    else
    {
        Console.WriteLine("Save file does not exist.");
        return null;
    }
}

LoadingFile();
//Run Program Type "1"
