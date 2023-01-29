using System.Net.Sockets;
using System.Text.Json;

namespace MM2p2msg;

public class User : IDisposable
{
    private const string FileName = "friends";
    private int _clientPort = 5050;
    private int _localServerPort = 5000;
    private List<Contacts>? _obj;
    public List<CancellationTokenSource> CtsS = null;
    public List<Server> Servers;

    public User()
    {
        CtsS = new List<CancellationTokenSource>();
        Servers = new List<Server>();
    }
    
    public static void SaveContactToJson(string nick, string ip, int port)
    {
        Contacts c = new Contacts(nick, ip, port, false, 0, null, null);
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

    public List<Contacts>? GetContactsFromJson()
    {
        string file;
        if ((file = File.ReadAllText(FileName)) == string.Empty)
        {
            throw new FileNotFoundException("No JSON file...");
        }
        
        _obj = JsonSerializer.Deserialize<List<Contacts>>(file);
        _localServerPort = 5000;
        return _obj;
    }

    // Mozna uzyc np getCdontactsFromJson(line -> funkcja(line)) funkcja zapisuje dane do tablicy
    public async Task<List<Contacts>> TryFriendlyContacts(Action<Contacts> newContact)
    {
        List<Task<Contacts>> tryes = new List<Task<Contacts>>();
        List<Contacts> output = new List<Contacts>();
        int iter = 0;
        if (_obj != null)
            foreach (var o in _obj)
            {
                Console.WriteLine(o.Ip);
                _clientPort++;
                o.C = new Client(o, CtsS?[iter], _localServerPort){TempPort = _clientPort};
                o.S = Servers[iter];
                o.C.ServerPort = o.S.Port;
                tryes.Add(o.C.TryConnect());
                _localServerPort++;
                iter++;
            }
        
        Contacts[] connections = await Task.WhenAll(tryes);
        foreach (Contacts connection in connections)
        {
            newContact(connection);
            output.Add(connection);
        }
        
        return output;
    }

    public async Task RunServer()
    {
        Server s = new Server(_localServerPort);
        Servers.Add(s);
        _localServerPort++;
        await s.GetMsg(CtsS[_localServerPort-5001].Token);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}