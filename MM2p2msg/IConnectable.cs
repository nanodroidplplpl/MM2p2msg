using System.Net.Sockets;

namespace MM2p2msg;

public interface IConnectable
{
    public int port { get; set; }
    public string host { get; set; }

    Socket socket { get; set; }
    public void createSocket();
}