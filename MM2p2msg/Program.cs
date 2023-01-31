// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2Pmsg
{
    static AutoResetEvent _updateGui = new AutoResetEvent(false);
    static AutoResetEvent _enableInput = new AutoResetEvent(false);
    private static CancellationTokenSource _endProgram = new CancellationTokenSource();
    static string UserName = "maciekP";

    static void MainServerThread(Server mainServer, CancellationToken endProgramToken)
    {
        Task mainServerTask = mainServer.MainServerTask(endProgramToken);
    }
    
    static async Task Main()
    {
        var endProgramToken = _endProgram.Token;
        User kUser = new User(UserName);
        _enableInput.Set();
        GuiMeneger guiMeneger = new GuiMeneger(UserName, _updateGui, _enableInput, UserName, kUser);
        //User.SaveContactToJson("Mati", "26.101.171.76", 5000);
        // Try contact to friends
        List<Contacts>? friends = kUser.GetContactsFromJson();
        friends = await kUser.TryFriendlyContacts(
            result => 
                guiMeneger._guis[0].PrintContacts(result, guiMeneger._top, guiMeneger.cardSelection));
        // Console.WriteLine("Dupa 1");
        MonitorServerGui monitorServerGui = new MonitorServerGui(friends); 
        // Console.WriteLine("Dupa 2");
        Server mainServer = new Server(5000, monitorServerGui, _updateGui);
        // Console.WriteLine("Dupa 3");
        guiMeneger.MonitorServerGui = monitorServerGui;
        // Console.WriteLine("Dupa 4");
        Task mainServerTask = mainServer.MainServerTask(endProgramToken);
        //Thread serverThread = new Thread(delegate() { MainServerThread(mainServer, endProgramToken); });
        //serverThread.Start();
        Task guiMenegerTask = Task.Run(() => { guiMeneger.GetUserInput(endProgramToken, _endProgram); });
        // Console.WriteLine("Dupa 7");
        // Task printUi = guiMeneger.PrintUi();
        Task printUi = Task.Run(() => { guiMeneger.PrintUi(endProgramToken); });
        await Task.WhenAll(mainServerTask, printUi, guiMenegerTask);
        //await Task.WhenAll(printUi, guiMenegerTask);
        //serverThread.Abort();
        Console.Clear();
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("| Dziekujemy za kozystanie z naszego retro komunikatora! |");
        Console.WriteLine("----------------------------------------------------------");
    }
}