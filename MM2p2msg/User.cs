using System.Text.Json;

namespace MM2p2msg;

public class User
{
    private const string FileName = "friends";

    public static void SaveContactToJson(string nick, string ip, int port)
    {
        Contacts c = new Contacts(){name = nick, ip = ip, port = port};
        string file;
        if ((file = File.ReadAllText(FileName)) == string.Empty)
        {
            List<Contacts> toJson = new List<Contacts>();
            toJson.Add(c);
            File.WriteAllText(FileName,JsonSerializer.Serialize(toJson));
        }
        else
        {
            List<Contacts>? fromJson = JsonSerializer.Deserialize<List<Contacts>>(file);
            fromJson?.Add(c);
            if (fromJson != null) File.WriteAllText(FileName, JsonSerializer.Serialize(fromJson));
        }
    }

    // Mozna uzyc np getCdontactsFromJson(line -> funkcja(line)) funkcja zapisuje dane do tablicy
    public void GetContactsFromJson(Action<Contacts> newContact)
    {
        string file;
        if ((file = File.ReadAllText(FileName)) == string.Empty)
        {
            throw new FileNotFoundException("No JSON file...");
        }
        List<Contacts>? obj = JsonSerializer.Deserialize<List<Contacts>>(file);
        if (obj != null)
            foreach (var o in obj)
            {
                newContact(o);
            }
    }
}