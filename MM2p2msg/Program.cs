// See https://aka.ms/new-console-template for more information
namespace MM2p2msg;

internal abstract class P2pmsg
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
        User k = new User();
        User.SaveContactToJson("maciek", "192.168.1.1", 5005);
        User.SaveContactToJson("mati", "192.168.1.2", 5005);
        Console.WriteLine("Zapisano do pliku");
        object record;
        k.GetContactsFromJson(record => Console.WriteLine("Nick: "+record.name+
                                                          " ip: "+record.ip+
                                                          " port: "+record.port.ToString()));
    }
}

// 2. odczyt zapis do pliku json asynchroniczne zapisywanie do tablicy i potem do pliku <- Mati
// 3. gui terminal <- Maciek