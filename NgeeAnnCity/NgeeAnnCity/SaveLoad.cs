using System.Text.Json;

public class SaveFile
{
    public string GameType {get; set;}
    public DateTime SaveDateTime {get; set;}
    public int SaveID {get; set;}
    public string? SaveDesc {get; set;}
    public string Location {get; set;}
    public string Stats {get; set;}

    public override string ToString()
    {
        return $"{GameType}\n{SaveDateTime}\n{SaveID}\n{SaveDesc}\n{Location}\n{Stats}";
    }
}