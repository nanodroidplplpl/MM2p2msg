// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2Pmsg
{
    static void Main()
    {
        User k = new User();
        UserInterface gui = new UserInterface("Maciek");
        User.SaveContactToJson("maciek1", "192.168.1.1", 5005);
        User.SaveContactToJson("mati2", "192.168.1.2", 5005);
        // User.SaveContactToJson("maciek3", "192.168.1.1", 5005);
        // User.SaveContactToJson("mati4", "192.168.1.2", 5005);
        // User.SaveContactToJson("maciek5", "192.168.1.1", 5005);
        // User.SaveContactToJson("mati6", "192.168.1.2", 5005);
        //Console.WriteLine("Zapisano do pliku");
        //object record;
        k.GetContactsFromJson(record => gui.PrintContacts(record));
        //Console.WriteLine("[1] "+gui.GetUserInput());
        string msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
    }
}

// 2. odczyt zapis do pliku json asynchroniczne zapisywanie do tablicy i potem do pliku <- Mati
// 3. gui terminal <- Maciek