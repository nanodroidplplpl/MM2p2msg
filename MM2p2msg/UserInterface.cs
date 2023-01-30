using System.Text.RegularExpressions;

namespace MM2p2msg;

public class UserInterface
{
    public int InputCursorPosX { get; set; }
    public int InputCursorPosY { get; set; }
    public int ListSize { get; set; }
    public int ListMaxSize { get; set; }
    public List<string> Output { get; set; }
    
    public string ip { get; set; }

    public string Name;

    public UserInterface(Contacts friend)
    {
        //Output = new List<string>();
        Output = friend.Conf;
        ip = friend.Ip;
        Console.Clear();
        //Console.WriteLine("Hello "+name);
        Output.Add(".NET chat, Nowy Elegancki Terminal NET");
        Output.Add("Konwersacja: ");
        Name = friend.Name;
        ListSize = 2;
        ListMaxSize = 7;
        Update(null, null);
    }

    public void PrintTop(List<string>? top, int? cardSelection)
    {
        int iter = 0;
        if (top != null)
            foreach (var t in top)
            {
                if (iter == cardSelection)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("["+t+"] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("["+t+"] ");
                    }
                }

                iter++;
            }
        Console.WriteLine();
    }

    public bool CheckForClones(Contacts c)
    {
        foreach (var outer in Output)
        {
            Match a = Regex.Match(outer, @"\[online\]\s+(.*)");
            Match b = Regex.Match(outer, @"\[offline\]\s+(.*)");
            
            if (a.Groups[1].Value == c.Name+" "+c.Ip+" "+c.Port.ToString() || b.Groups[1].Value == c.Name+" "+c.Ip+" "+c.Port.ToString())
            {
                return true;
            }
        }

        return false;
    }
    
    public void PrintContacts(Contacts c, List<string> top, int cardSelection)
    {
        string color = null;
        if (c.Active)
            color = "[online] ";
        else
            color = "[offline] ";
        if (!CheckForClones(c))
        {
            SendGui(color+c.Name+" "+c.Ip+" "+c.Port.ToString());
            Update(top, cardSelection);
        }
    }

    public void SendGui(string msg)
    {
        string temp;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        // if (Output.Count < ListMaxSize)
        //     Output.Add(msg);
        // else
        // {
        //     for (int i = 2; i < Output.Count-1; i++)
        //     {
        //         Output[i] = Output[i + 1];
        //     }
        //
        //     Output[Output.Count - 1] = msg;
        // }
        Output.Add(msg);
    }

    public void Update(List<string>? top, int? cardSelection)
    {
        Console.Clear();
        Console.CursorTop = 0;
        Console.CursorLeft = 0;
        PrintTop(top, cardSelection);
        // foreach (var line in Output)
        // {
        //     Console.WriteLine(line);
        // }
        if (Output.Count < 6)
        {
            foreach (var line in Output)
            {
                Console.WriteLine(line);
            }

            return;
        }
        for (int i = Output.Count - 6; i < Output.Count; i++)
        {
            Console.WriteLine(Output[i]);
        }
    }
}