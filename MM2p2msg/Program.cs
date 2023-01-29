// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2Pmsg
{
    static async Task Main()
    {
        User kUser = new User();
        UserInterface gui = new UserInterface("Maciej");
        //User.SaveContactToJson("maciek1", "192.168.1.1", 5005);
        //User.SaveContactToJson("mati2", "192.168.1.2", 5005);
        //User.SaveContactToJson("maciekPC", "127.0.0.1", 5001);
        User.SaveContactToJson("maciekP", "127.0.0.1", 5001);
        // Try contact to friends
        List<Contacts>? friends = kUser.GetContactsFromJson();
        List<Task> servers = new List<Task>();
        if (friends != null)
            foreach (var friend in friends)
            {
                kUser.CtsS?.Add(new CancellationTokenSource());
                servers.Add(kUser.RunServer());
            }

        friends = await kUser.TryFriendlyContacts(result => gui.PrintContacts(result));
        string? msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        msg = gui.GetUserInput();
        Console.WriteLine("Dupa 1");
        foreach (var server in servers)
        {
            if (server.IsCanceled)
            {
                Console.WriteLine("Skasowany dupa");
            }
            else
            {
                Console.WriteLine("działa");
            }
        }
        await Task.WhenAll(servers);
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