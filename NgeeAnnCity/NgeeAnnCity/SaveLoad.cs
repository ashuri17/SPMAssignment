using System.IO;
using System.Text.Json;

public class SaveFile
{
    public string GameType {get; set;}
    public DateTime SaveDateTime {get; set;}
    public int SaveID {get; set;}
    public string? SaveDesc {get; set;}
    public string Location {get; set;}
    public string Stats {get; set;}
    public string FilePath {get; set;}

    private static SaveFile? LoadFile()
    {
        string saveFile;

        // directory path for save files
        string saveFilesDirectory = "";

        // get save files in directory
        string[] saveFiles = Directory.GetFiles(saveFilesDirectory);

        // check if directory is empty
        if (!saveFiles.Any())
        {
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
                Console.WriteLine("That savefile does not exist.");
                continue;
            }

            saveFile = saveFiles[result];
            break;
        }

        // Jian Hui's code
        using (StreamReader r = new StreamReader(saveFile)) //Reading file
        {
            string jsonString = r.ReadToEnd();
            return JsonSerializer.Deserialize<SaveFile>(jsonString);
        }


    }

    public override string ToString()
    {
        return $"{GameType}\n{SaveDateTime}\n{SaveID}\n{SaveDesc}\n{Location}\n{Stats}";
    }
}