using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MM2p2msg;

public class Server : IConnectable
{
    public int Port { get; set; }
    public string Host { get; set; }
    public Socket Socket { get; set; }

    public MonitorServerGui ServerGuiConnect;
    private AutoResetEvent _updateGui;

    public Server(int port, MonitorServerGui serverGuiConnect, AutoResetEvent updateGui)
    {
        Port = port;
        ServerGuiConnect = serverGuiConnect;
        _updateGui = updateGui;
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

    public void MakeSomethingWithMsg(IPAddress address, string msg)
    {
        string Saddress = address.ToString();
        Match match = Regex.Match(msg, @":AscConnection");
        List<Contacts> monitoredVar = (List<Contacts>)ServerGuiConnect.GetMonitoredVar();
        Console.WriteLine("Dodaje dla: "+msg);
        if (!match.Success)
        {
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress)
                {
                    mVar.Conf.Add(mVar.Name+msg);
                    //Debug.WriteLine("Dodaje dla: "+mVar.Name);
                    // using (StreamWriter sw = File.CreateText(@"dupa_kurwa_dupa_jebana1.txt"))
                    // {
                    //     sw.WriteLine("Hello World");
                    // }
                }
            }
        }
        else // W takim razie pytanie o polaczenie
        {
            //string name, string ip, int port, bool active, int localPort, Client c, Server s
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress)
                {
                    mVar.Active = true;
                    _updateGui.Set();
                    return;
                }
            }
            // W takim razie nieznajomy
            Match mat = Regex.Match(msg, @"(\w):");
            monitoredVar.Add(new Contacts(mat.Groups[1].Value, Saddress, 5000, true, 6600, null, null));
        }
        ServerGuiConnect.SetMonitoredVar(monitoredVar);
        _updateGui.Set();
    }

    public async Task MainServerTask()
    {
        IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localAddress = IPAddress.Parse("26.129.155.17");//host.AddressList[0];

        var listener = new TcpListener(localAddress, 5000);
        listener.Start();
        //Console.WriteLine("Wstalo");
        List<Task> tasks = new List<Task>();
        
        while (true)
        {
            var client = listener.AcceptTcpClient();
            var task = Task.Factory.StartNew(() =>
            {
                var stream = client.GetStream();
                var clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint)?.Address;
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.WriteLine("Otrzymano ip: "+message);
                    if (clientIpAddress != null) MakeSomethingWithMsg(clientIpAddress, message);
                }
            });
            tasks.Add(task);
        }
        //await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        Socket.Dispose();
    }
}