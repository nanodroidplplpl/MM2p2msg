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
    private string UserIp;

    public Server(int port, MonitorServerGui serverGuiConnect, AutoResetEvent updateGui, string userIp)
    {
        Port = port;
        ServerGuiConnect = serverGuiConnect;
        _updateGui = updateGui;
        UserIp = userIp;
    }
    public Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task GetMsg(CancellationToken ctsToken)
    {
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

    public void MakeSomethingWithMsg(IPAddress address, string msg, int port)
    {
        string Saddress = address.ToString();
        Match match = Regex.Match(msg, @":AscConnection");
        List<Contacts> monitoredVar = (List<Contacts>)ServerGuiConnect.GetMonitoredVar();
        if (!match.Success)
        {
            Match mat = Regex.Match(msg, @"(\w+):");
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress && mVar.Name == mat.Groups[0].Value)
                {
                    mVar.Conf.Add(msg);
                }
            }
        }
        else 
        {
            Match ma = Regex.Match(msg, @"-(\d+)");
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress && mVar.Port == int.Parse(ma.Groups[0].Value))
                {
                    mVar.Active = true;
                    ServerGuiConnect.SetMonitoredVar(monitoredVar);
                    _updateGui.Set();
                    return;
                }
            }
            Match mat = Regex.Match(msg, @"(\w+):");
            monitoredVar.Add(new Contacts(mat.Groups[1].Value, Saddress, int.Parse(ma.Groups[0].Value), true, 6600, null, null));
        }
        ServerGuiConnect.SetMonitoredVar(monitoredVar);
        _updateGui.Set();
    }

    public async Task MainServerTask(CancellationToken endProgram)
    {
        IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localAddress = IPAddress.Parse(UserIp); 

        var listener = new TcpListener(localAddress, 5000);
        listener.Start();
        List<Task> tasks = new List<Task>();
        listener.Server.ReceiveTimeout = 100;
        listener.Server.SendTimeout = 100;
        while (!endProgram.IsCancellationRequested)
        {
            if (!listener.Pending())
            {
                Thread.Sleep(100);
                continue;
            }
            var client = listener.AcceptTcpClient();
            var task = Task.Factory.StartNew(() =>
            {
                var stream = client.GetStream();
                var clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint)?.Address;
                string portCl = ((IPEndPoint)client.Client.RemoteEndPoint)?.Port.ToString();
                var buffer = new byte[1024];
                int bytesRead;
                while (!endProgram.IsCancellationRequested)
                {
                    if (!stream.DataAvailable)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.WriteLine("Otrzymano ip: " + message);
                    if (clientIpAddress != null) MakeSomethingWithMsg(clientIpAddress, message, int.Parse(portCl));
                }
            }, endProgram);
            tasks.Add(task);
        }
        listener.Stop();
        Console.WriteLine("Kończe server");
    }
}