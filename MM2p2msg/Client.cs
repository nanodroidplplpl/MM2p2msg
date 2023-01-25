using System.Net;
using System.Net.Mail;
using System.Net.Sockets;

namespace MM2p2msg;

public class Client : IConnectable
{
    public string host { get; set; }
    public Socket socket { get; set; }
    public int port { get; set; }
    
    // Utworzyc tablice na znajome hosty
    
    public Client(string host, int port)
    {
        host = host;
        port = port;
    }

    public void createSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ip = IPAddress.Parse(host);

        IPEndPoint endPoint = new IPEndPoint(ip, port);
    }

    // Zrobic zbieranie informacji o kontaktach (nick, host_ip, host_port) z pliku json, async!!!
    public void getFriendlyHosts()
    {
        
    }

    public void tryConnect()
    {
        
    }
}