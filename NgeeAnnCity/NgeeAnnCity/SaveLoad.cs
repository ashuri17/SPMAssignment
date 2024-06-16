using System.Text.Json;

public class SaveFile
{
    public DateTime SaveDateTime {get; set;}
    public int SaveID {get; set;}
    public string? SaveDesc {get; set;}
    public string Location {get; set;}
    public string Stats {get; set;}
    public string FilePath {get; set;}
}