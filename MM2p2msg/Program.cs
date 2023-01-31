// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2Pmsg
{
    static AutoResetEvent _updateGui = new AutoResetEvent(false);
    static AutoResetEvent _enableInput = new AutoResetEvent(false);
    static string UserName = "maciekP";
    static async Task Main()
    {
        User kUser = new User(UserName);
        _enableInput.Set();
        GuiMeneger guiMeneger = new GuiMeneger(UserName, _updateGui, _enableInput, UserName, kUser);
        //UserInterface gui = new UserInterface("Maciej");
        //User.SaveContactToJson("maciek1", "192.168.1.1", 5005);
        //User.SaveContactToJson("mati2", "192.168.1.2", 5005);
        //User.SaveContactToJson("maciekPC", "127.0.0.1", 5001);
        //User.SaveContactToJson("maciekP", "127.0.0.1", 5000);
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
        Task mainServerTask = mainServer.MainServerTask();
        //Task mainServerTask = Task.Run(() => { mainServer.MainServerTask(); });
        // Console.WriteLine("Dupa 5");
        
        // friends = await kUser.TryFriendlyContacts(
        //     result => 
        //         guiMeneger._guis[0].PrintContacts(result, guiMeneger._top, guiMeneger.cardSelection));
        //Task printUi = guiMeneger.PrintUi();
        // Task printUi = guiMeneger.PrintUi();
        // MonitorServerGui monitorServerGui = new MonitorServerGui(friends);
        // Server mainServer = new Server(5000, monitorServerGui, _updateGui);
        // guiMeneger.MonitorServerGui = monitorServerGui;
        // Task mainServerTask = mainServer.MainServerTask();
        // Console.WriteLine("Dupa 6");
        // Task guiMenegerTask = guiMeneger.GetUserInput();
        Task guiMenegerTask = Task.Run(() => { guiMeneger.GetUserInput(); });
        // Console.WriteLine("Dupa 7");
        // Task printUi = guiMeneger.PrintUi();
        Task printUi = Task.Run(() => { guiMeneger.PrintUi(); });
        await Task.WhenAll(mainServerTask, printUi, guiMenegerTask);
    }
}

// 2. odczyt zapis do pliku json asynchroniczne zapisywanie do tablicy i potem do pliku <- Mati
// 3. gui terminal <- Maciek

// User k = new User();
// UserInterface gui = new UserInterface("Maciek");
// // User.SaveContactToJson("maciek1", "192.168.1.1", 5005);
// // User.SaveContactToJson("mati2", "192.168.1.2", 5005);
// k.GetContactsFromJson(record => gui.PrintContacts(record));
// //Console.WriteLine("[1] "+gui.GetUserInput());
// string msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();
// msg = gui.GetUserInput();