using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace MM2p2msg;

public class Server : IConnectable
{
    public int Port { get; set; }
    public string Host { get; set; }
    public Socket Socket { get; set; }

    public Server(int port)
    {
        Port = port;
    }
    public Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task GetMsg(CancellationToken ctsToken)
    {
        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
        Socket = CreateSocket();
        Socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port));

        Socket.Listen();

        while (true)
        {
            Socket clientSocket = await Socket.AcceptAsync(ctsToken);
            byte[] buffer = new byte[1024];
            int bytesReceived = clientSocket.Receive(buffer);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
        }
    }

    public async Task MainServerTask()
    {
        IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localAddress = host.AddressList[0];

        var listener = new TcpListener(localAddress, 6000);
        listener.Start();
        Console.WriteLine("Wstalo");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            await Task.Factory.StartNew(() =>
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Otrzymano ip: "+message);
                    Socket choiceServerPort = CreateSocket();
                    
                }
            });
        }
    }

    public void Dispose()
    {
        Socket.Dispose();
    }
}