using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MM2p2msg;

public class Client
{
    private Contacts Contact { get; set; }
    public Socket Socket { get; set; } = null!;
    //public int TempPort = 5050;
    public string usrName;

    public Client(Contacts contact, string usrName)
    {
        Contact = contact;
        this.usrName = usrName;
    }

    public Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<Contacts> TryConnect(int myPort)
    {
        IPAddress ip = IPAddress.Parse(Contact.Ip);
        IPEndPoint endPoint = new IPEndPoint(ip, Contact.Port);
    
        CancellationTokenSource ctsClient = new CancellationTokenSource();
        CancellationToken ctClient = ctsClient.Token;
        ctsClient.CancelAfter(1500);
    
        try
        {
            using (var socket = CreateSocket())
            {
                socket.SendTimeout = 1;
                socket.ReceiveTimeout = 1;
                await socket.ConnectAsync(endPoint, ctClient);
                Contact.Active = true;

                byte[] buffer = Encoding.ASCII.GetBytes(usrName+":AscConnection:"+myPort);
                Debug.WriteLine("Wysylam probe polaczenia do Mati");
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                // args.RemoteEndPoint = endPoint;
                // args.SetBuffer(buffer, 0, buffer.Length);
                // socket.SendAsync(args);
                socket.Send(buffer);
            }
        }
        catch (Exception)
        {
            Contact.Active = false;
            return Contact;
        }

        return Contact;
    }

    public bool SendMessage(string msg)
    {
        byte[] msgSerialized = Encoding.ASCII.GetBytes(msg);
        IPAddress ip = IPAddress.Parse(Contact.Ip);
        IPEndPoint endPoint = new IPEndPoint(ip, Contact.Port);
        CancellationTokenSource ctsClient = new CancellationTokenSource();
        CancellationToken ctClient = ctsClient.Token;
        ctsClient.CancelAfter(4000);
        try
        {
            using (var socket = CreateSocket())
            {
                socket.Connect(endPoint);
                socket.Send(msgSerialized);
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}