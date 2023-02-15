using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MM2p2msg;

public class Server
{
    public int Port { get; set; }
    public string Host { get; set; }
    public Socket Socket { get; set; }

    public MonitorServerGui ServerGuiConnect;
    private AutoResetEvent _updateGui;
    private string UserIp;
    private int UserPort;

    public Server(int port, MonitorServerGui serverGuiConnect, AutoResetEvent updateGui, string userIp, int userPort)
    {
        Port = port;
        ServerGuiConnect = serverGuiConnect;
        _updateGui = updateGui;
        UserIp = userIp;
        UserPort = userPort;
    }
    public Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void MakeSomethingWithMsg(IPAddress address, string msg, int port)
    {
        string Saddress = address.ToString();
        Match match = Regex.Match(msg, @":AscConnection");
        List<Contacts> monitoredVar = (List<Contacts>)ServerGuiConnect.GetMonitoredVar();
        if (!match.Success)
        {
            Match mat = Regex.Match(msg, @"(\w+):");
            //Debug.WriteLine(msg);
            //Debug.WriteLine("Wiadomosc od: "+mat.Groups[1].Value);
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress && mVar.Name == mat.Groups[1].Value)
                {
                    mVar.Conf.Add(msg);
                }
            }
        }
        else 
        {
            Match ma = Regex.Match(msg, @":(\d+)");
            foreach (var mVar in monitoredVar)
            {
                if (mVar.Ip == Saddress && mVar.Port == int.Parse(ma.Groups[1].Value))
                {
                    mVar.Active = true;
                    ServerGuiConnect.SetMonitoredVar(monitoredVar);
                    _updateGui.Set();
                    Debug.WriteLine(msg);
                    return;
                }
            }
            Match mat = Regex.Match(msg, @"(\w+):");
            monitoredVar.Add(new Contacts(mat.Groups[1].Value, Saddress, int.Parse(ma.Groups[1].Value), true, 0, null, null));
        }
        ServerGuiConnect.SetMonitoredVar(monitoredVar);
        _updateGui.Set();
    }

    public async Task MainServerTask(CancellationToken endProgram)
    {
        IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localAddress = IPAddress.Parse(UserIp);

        var listener = new TcpListener(localAddress, UserPort);
        listener.Start();
        //listener.Start();
        List<Task> tasks = new List<Task>();
        while (!endProgram.IsCancellationRequested)
        {
            //var client = listener.AcceptTcpClient();
            //var client = await listener.AcceptTcpClientAsync(endProgram);
            var client = await listener.AcceptTcpClientAsync(endProgram)
                .AsTask()
                .ContinueWith(task => task.IsCanceled ? null : task.Result);

            if (client is not null)
            {
                var task = Task.Factory.StartNew(async () =>
                {
                    //var stream = client.GetStream();
                    using (var stream = client.GetStream())
                    {
                        var clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint)?.Address;
                        string portCl = ((IPEndPoint)client.Client.RemoteEndPoint)?.Port.ToString();
                        var buffer = new byte[1024*8];
                        int bytesRead;
                        while (!endProgram.IsCancellationRequested)
                        {
                            if ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, endProgram)) != 0)
                            {
                                Debug.Write(buffer);
                                var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                Debug.WriteLine("Otrzymano ip: " + message);
                                if (clientIpAddress != null) MakeSomethingWithMsg(clientIpAddress, message, int.Parse(portCl));
                            }
                        }
                    }
                    client.Dispose();
                }, endProgram);
                tasks.Add(task);
            }
        }
        //listener.Stop();
        //Console.WriteLine("Kończe server");
    }
}