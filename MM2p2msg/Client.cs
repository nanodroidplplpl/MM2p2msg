using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MM2p2msg;

public class Client : IConnectable
{
    private Contacts Contact { get; set; }
    public Socket Socket { get; set; } = null!;
    public int TempPort = 5050;
    public string usrName;

    // Utworzyc tablice na znajome hosty
    
    public Client(Contacts contact, string usrName)
    {
        Contact = contact;
        this.usrName = usrName;
    }

    public Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<Contacts> TryConnect()
    {
        Socket = CreateSocket();
        Socket.SendTimeout = 1;
        Socket.ReceiveTimeout = 1;
        IPAddress ip = IPAddress.Parse(Contact.Ip);
        IPEndPoint endPoint = new IPEndPoint(ip, Contact.Port);
        //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _tempPort);
        //socket.Bind(localEndPoint);

        CancellationTokenSource ctsClient = new CancellationTokenSource();
        CancellationToken ctClient = ctsClient.Token;
        ctsClient.CancelAfter(1500);
        
        try
        {
            await Socket.ConnectAsync(endPoint, ctClient);
            Contact.Active = true;
            //Cts.Cancel();
        }
        catch (Exception)
        {
            Contact.Active = false;
            return Contact;
        }
        Contact.Active = true;
        byte[] buffer = Encoding.ASCII.GetBytes(usrName+":AscConnection");
        Debug.WriteLine("Wysylam probe polaczenia do Mati");
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.RemoteEndPoint = endPoint;
        args.SetBuffer(buffer, 0, buffer.Length);
        Socket.SendAsync(args);
        return Contact;
    }

    public bool SendMessage(string msg)
    {
        byte[] msgSerialized = Encoding.ASCII.GetBytes(msg);
        Socket = CreateSocket();
        IPAddress ip = IPAddress.Parse(Contact.Ip);
        IPEndPoint endPoint = new IPEndPoint(ip, Contact.Port);
        CancellationTokenSource ctsClient = new CancellationTokenSource();
        CancellationToken ctClient = ctsClient.Token;
        ctsClient.CancelAfter(4000);
        try
        {
            Socket.Connect(endPoint);
        }
        catch (Exception)
        {
            return false;
        }

        Socket.Send(msgSerialized);
        return true;
    }

    public void Dispose()
    {
        Socket.Dispose();
    }
}