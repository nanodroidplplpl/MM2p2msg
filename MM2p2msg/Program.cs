// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2Pmsg
{
    static AutoResetEvent _updateGui = new AutoResetEvent(false);
    static AutoResetEvent _enableInput = new AutoResetEvent(false);
    private static CancellationTokenSource _endProgram = new CancellationTokenSource();
    private static string UserName;
    private static string UserIP;
    private static int UserPort = 5000;

    static async Task Main()
    {
        Console.WriteLine("----------------------------------------------------------");
        Console.Write("Podziel sie swoim imieniem: ");
        UserName = Console.ReadLine();
        Console.Write("Podaj ip swojego komputera: ");
        UserIP = Console.ReadLine();
        Console.Write("Czy chcialbys zmienic nr portu, domyslnie 5000 T/n: ");
        if (Console.ReadLine() == "T")
        {
            Console.Write("Podaj nr portu: ");
            UserPort = int.Parse(Console.ReadLine()!);
        }
        var endProgramToken = _endProgram.Token;
        User kUser = new User(UserName);
        _enableInput.Set();
        GuiMeneger guiMeneger = new GuiMeneger(UserName, _updateGui, _enableInput, UserName, kUser, UserPort);
        List<Contacts>? friends = kUser.GetContactsFromJson();
        if (friends.Count != 0)
        {
            friends = await kUser.TryFriendlyContacts(
                result => 
                    guiMeneger._guis[0].PrintContacts(result, guiMeneger._top, guiMeneger.cardSelection), UserPort);
        }
        else
        {
            guiMeneger._guis[0].Update(guiMeneger._top, guiMeneger.cardSelection);
        }
        friends = kUser.ReadFrom(friends);
        MonitorServerGui monitorServerGui = new MonitorServerGui(friends);
        Server mainServer = new Server(UserPort, monitorServerGui, _updateGui, UserIP, UserPort);
        guiMeneger.MonitorServerGui = monitorServerGui;
        Task mainServerTask = mainServer.MainServerTask(endProgramToken);
        Task guiMenegerTask = Task.Run(() => { guiMeneger.GetUserInput(endProgramToken, _endProgram); });
        Task printUi = Task.Run(() => { guiMeneger.PrintUi(endProgramToken); });
        await Task.WhenAll(mainServerTask, printUi, guiMenegerTask);
        Console.Clear();
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("| Dziekujemy za kozystanie z naszego retro komunikatora! |");
        Console.WriteLine("----------------------------------------------------------");
    }
}