namespace MM2p2msg;

public class ControlUI : UserInterface
{
    public ControlUI(string name) : base(name){}

    public new string? GetUserInput()
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
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        return msg;
    }
}