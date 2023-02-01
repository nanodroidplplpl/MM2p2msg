using System.Net.Sockets;
using System.Runtime.Serialization;

namespace MM2p2msg;

[DataContract]
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

    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Ip { get; set; }
    [DataMember]
    public int Port { get; set; }
    
    public int LocalPort { get; set; }
    
    public bool Active { get; set; }
    
    public Client C { get; set; }
    
    public Server S { get; set; }

    public List<string> Conf;
}