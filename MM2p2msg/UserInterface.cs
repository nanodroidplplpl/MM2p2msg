namespace MM2p2msg;

public class UserInterface
{
    public int InputCursorPosX { get; set; }
    public int InputCursorPosY { get; set; }
    public int ListSize { get; set; }
    public int ListMaxSize { get; set; }
    public List<string> Output { get; set; }
    
    public UserInterface(string name)
    {
        Output = new List<string>();
        Console.Clear();
        Console.WriteLine("Hello "+name);
        Output.Add(".NET chat, Nowy Elegancki Terminal NET");
        Output.Add("Hello Maciej");
        ListSize = 2;
        ListMaxSize = 7;
        Update();
    }
    
    public void PrintContacts(Contacts c)
    {
        string color = null;
        if (c.Active)
            color = "[online] ";
        else
            color = "[offline] ";
        SendGui(color+c.Name+" "+c.Ip+" "+c.Port.ToString());
        Update();
    }

    public void SendGui(string msg)
    {
        string temp;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        if (Output.Count < ListMaxSize)
            Output.Add(msg);
        else
        {
            for (int i = 2; i < Output.Count-1; i++)
            {
                Output[i] = Output[i + 1];
            }

            Output[Output.Count - 1] = msg;
        }
    }

    public void Update()
    {
        Console.Clear();
        Console.CursorTop = 0;
        Console.CursorLeft = 0;
        foreach (var line in Output)
        {
            Console.WriteLine(line);
        }
    }

    public string? GetUserInput()
    {
        Console.CursorTop += 5;
        Console.CursorLeft = 0;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        Console.Write("->");
        for (int i = Console.CursorLeft; i < 50; i++)
        {
            Console.CursorLeft = i;
            Console.Write(" ");
        }
        Console.CursorLeft = 0;
        Console.CursorLeft += 5;
        string? msg = Console.ReadLine();
        Console.CursorTop -= 1;
        if (msg != null)
            for (int i = 0; i < msg.Length; i++)
            {
                Console.CursorLeft = i;
                Console.Write(" ");
            }

        Console.CursorLeft = 0;
        SendGui(msg);
        Update();
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        return msg;
    }
}