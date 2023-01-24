using System.Net.Sockets;

namespace MM2p2msg;

public class Server : IConnectable
{
    public int port { get; set; }
    public string host { get; set; }
    public Socket socket { get; set; }
    
    public void CreateSocket()
    {
        throw new NotImplementedException();
    }
}