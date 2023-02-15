using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MM2p2msg;

public class Contacts
{
    public Contacts(string name, string ip, int port, bool active, int localPort, Client c, Server s)
    {
        this.Name = name;
        Ip = ip;
        Port = port;
        Active = active;
        LocalPort = LocalPort;
        C = c;
        S = s;
        Conf = new List<string>();
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("ip")]
    public string Ip { get; set; }
    [JsonPropertyName("port")]
    public int Port { get; set; }
    [JsonIgnore]
    public int LocalPort { get; set; }
    
    [JsonIgnore]
    public bool Active { get; set; }
    
    [JsonIgnore]
    public Client C { get; set; }
    
    [JsonIgnore]
    public Server S { get; set; }
    
    [JsonIgnore]
    public List<string> Conf;
}