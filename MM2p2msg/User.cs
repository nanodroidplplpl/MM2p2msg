using System.Diagnostics;
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
    private string usrName;


    public User(string usrName)
    {
        CtsS = new List<CancellationTokenSource>();
        this.usrName = usrName;
    }
    
    public void SaveContactToJson(string nick, string ip, int port)
    {
        Contacts c = new Contacts(nick, ip, port, false, 0, null, null);
        string file;
        if (!File.Exists(FileName))//((file = File.ReadAllText(FileName)) == string.Empty)
        {
            List<Contacts> toJson = new List<Contacts>();
            toJson.Add(c);
            File.WriteAllText(FileName,JsonSerializer.Serialize(toJson));
        }
        else if ((file = File.ReadAllText(FileName)) == string.Empty && File.Exists(FileName))
        {
            List<Contacts> toJson = new List<Contacts>();
            toJson.Add(c);
            File.WriteAllText(FileName,JsonSerializer.Serialize(toJson));
        }
        else
        {
            List<Contacts>? fromJson = JsonSerializer.Deserialize<List<Contacts>>(file = File.ReadAllText(FileName));
            fromJson?.Add(c);
            if (fromJson != null) File.WriteAllText(FileName, JsonSerializer.Serialize(fromJson));
        }
    }

    public void SaveConotactToJsonOnExit(List<Contacts> contactsList)
    {
        foreach (var contact in contactsList)
        {
            contact.Active = false;
            contact.C = null;
            contact.S = null;
        }
        File.WriteAllText(FileName,JsonSerializer.Serialize(contactsList));
    }

    public List<Contacts>? GetContactsFromJson()
    {
        string file;
        if (File.Exists(FileName))
        {
            if (!((file = File.ReadAllText(FileName)) == string.Empty))
            {
                _obj = JsonSerializer.Deserialize<List<Contacts>>(file);
                _localServerPort = 5000;
                return _obj;
            }
        }
        else
        {
            return new List<Contacts>();
        }

        return new List<Contacts>();
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
                //Console.WriteLine(o.Ip);
                _clientPort++;
                o.C = new Client(o, usrName){TempPort = _clientPort};
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

    public void SaveConf(List<Contacts> toFile)
    {
        foreach (var file in toFile)
        {
            using (StreamWriter sw = new StreamWriter(file.Name, false))
            {
                //List<string> confList = new List<string>();
                foreach (var con in file.Conf)
                {
                    sw.WriteLine(con);
                }
            }
        }
    }

    public List<Contacts> ReadFrom(List<Contacts> fromFile)
    {
        foreach (var file in fromFile)
        {
            if (File.Exists(file.Name))
            {
                string file_stuff;
                using (var streamReader = new StreamReader(file.Name))
                {
                    file_stuff = streamReader.ReadToEnd();
                    if (file_stuff != string.Empty)
                    {
                        List<(string, int)> confList = new List<(string, int)>();
                        string[] lines = file_stuff.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        int iter = 0;
                        foreach (string l in lines)
                        {
                            confList.Add((l,iter));
                            iter++;
                        }
                        confList.RemoveAt(iter-1);
                        var topFive = confList.OrderByDescending(c => c.Item2).Take(5).Reverse();
                        foreach (var conf in topFive)
                        {
                            file.Conf.Add(conf.Item1);
                        }
                    }
                }
            }
        }
        return fromFile;
    }

    public async Task RunServer()
    {
        // Server s = new Server(_localServerPort);
        // Servers.Add(s);
        // _localServerPort++;
        // await s.GetMsg(CtsS[_localServerPort-5001].Token);
        
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}