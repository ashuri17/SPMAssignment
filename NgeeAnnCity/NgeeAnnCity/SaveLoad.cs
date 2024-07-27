using NgeeAnnCity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

public class PointCharDictionaryConverter : JsonConverter<Dictionary<Point, char>>
{
    public override Dictionary<Point, char> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dictionary = new Dictionary<Point, char>();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            var parts = propertyName.Split(',');
            Point point = new Point(int.Parse(parts[0]), int.Parse(parts[1]));

            reader.Read();
            char value = reader.GetString()[0];

            dictionary[point] = value;
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Point, char> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName($"{kvp.Key.X},{kvp.Key.Y}");
            writer.WriteStringValue(kvp.Value.ToString());
        }

        writer.WriteEndObject();
    }
}
public class SaveFile
{
    public DateTime SaveDateTime = DateTime.Now;
    public int ArcadeSaveID = Directory.GetFiles("ArcadeSave").Count() + 1;
    public int FreeplaySaveID = Directory.GetFiles("FreeplaySave").Count() + 1;
    public string? SaveDesc { get; set; }
    public static bool IsArcade { get; set; } // arcade true, freeplay false
    public static void LoadScreen(bool gameModePick)
    {
        string[] saveFiles;
        // directory path for save files
        if (gameModePick)
        {
            // get save files in directory
            string saveFilesDirectory = "ArcadeSave";
            saveFiles = Directory.GetFiles(saveFilesDirectory);
            IsArcade = true;
        }
        else
        {
            // get save files in directory
            string saveFilesDirectory = "FreeplaySave";
            saveFiles = Directory.GetFiles(saveFilesDirectory);
            IsArcade = false; 
        }


        // check if directory is empty
        if (!saveFiles.Any())
        {
            Console.WriteLine("No save file available");
            return;
        }

        // print save files for user to choose
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"[{i}] {saveFiles[i]}");
        }
        GameParser(saveFiles);
    }

    public static void GameParser(string[] saveFiles)
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
            string jsonString = File.ReadAllText(saveFiles[result]);
            File.Delete(saveFiles[result]);
            if (IsArcade)
            {
                LoadArcade(jsonString).PlayGame();
                break;
            }
            else
            {
                LoadFreePlay(jsonString).PlayGame();
                break;
            }
        }
    }

    public static Arcade LoadArcade(string saveFile)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new PointCharDictionaryConverter() },
            WriteIndented = true
        };
        Arcade loadedGameFile = JsonSerializer.Deserialize<Arcade>(saveFile,options);
        return loadedGameFile;
    }

    public static FreePlayGame LoadFreePlay(string saveFile)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new PointCharDictionaryConverter() },
            WriteIndented = true
        };
        FreePlayGame loadedGameFile = JsonSerializer.Deserialize<FreePlayGame>(saveFile,options);
        return loadedGameFile;
    }
}

public class ArcadeSaveFile : SaveFile
{
    public Arcade GameData { get; set; }

    public ArcadeSaveFile(Arcade gD, string sDesc)
    {
        GameData = gD;
        SaveDesc = sDesc;
    }

    // Takes Arcade class object, appends additional required data to the end and converts values to a json to be stored in a file
    public void CreateJsonFile()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new PointCharDictionaryConverter() },
            WriteIndented = true
        };

        var gameDataJson = JsonSerializer.Serialize(GameData, options);
        var dataJson = new List<string> { gameDataJson };

        using (StreamWriter sw = new StreamWriter($"ArcadeSave\\Arcade{ArcadeSaveID}.json", false))
        {
            foreach (string s in dataJson)
            {
                sw.WriteLine(s);
            }
        }
    }
}

public class FreePlaySaveFile : SaveFile
{
    public FreePlayGame GameData { get; set; }

    public FreePlaySaveFile(FreePlayGame gD, string sDesc)
    {
        GameData = gD;
        SaveDesc = sDesc;
    }

    public void CreateJsonFile()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new PointCharDictionaryConverter() },
            WriteIndented = true
        };

        var gameDataJson = JsonSerializer.Serialize(GameData, options);
        var dataJson = new List<string> { gameDataJson };

        using (StreamWriter sw = new StreamWriter($"FreeplaySave\\Freeplay{FreeplaySaveID}.json", false))
        {
            foreach (string s in dataJson)
            {
                sw.WriteLine(s);
            }
        }
    }
}
