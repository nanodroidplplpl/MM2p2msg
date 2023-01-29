using System.Net;
using System.Net.Sockets;

namespace MM2p2msg;

public interface IConnectable : IDisposable
{
    Socket Socket { get; set; }
    public Socket CreateSocket();
}