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
    
    public int port { get; set; }

    public UserInterface(Contacts friend)
    {
        Output = friend.Conf;
        ip = friend.Ip;
        Console.Clear();
        Name = friend.Name;
        ListSize = 2;
        ListMaxSize = 7;
        port = friend.Port;
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
                    //Console.ForegroundColor = ConsoleColor.Black;
                    //Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("[*"+t+"*] ");
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    {
                        //Console.ForegroundColor = ConsoleColor.White;
                        //Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("["+t+"] ");
                    }
                }

                iter++;
            }
        Console.WriteLine();
    }

    public void CheckForClones(Contacts c)
    {
        foreach (var outer in Output)
        {
            Match a = Regex.Match(outer, @"\[online\]\s+(.*)");
            Match b = Regex.Match(outer, @"\[offline\]\s+(.*)");
            
            if (a.Groups[1].Value == c.Name+" "+c.Ip+" "+c.Port.ToString())
            {
                Output.Remove("[online] "+c.Name+" "+c.Ip+" "+c.Port.ToString());
                return;
            }
            else if (b.Groups[1].Value == c.Name + " " + c.Ip + " " + c.Port.ToString())
            {
                Output.Remove("[offline] "+c.Name+" "+c.Ip+" "+c.Port.ToString());
                return;
            }
        }
    }
    
    public void PrintContacts(Contacts c, List<string> top, int cardSelection)
    {
        string color = null;
        if (c.Active)
            color = "[online] ";
        else
            color = "[offline] ";
        CheckForClones(c);
        SendGui(color+c.Name+" "+c.Ip+" "+c.Port);
        Update(top, cardSelection);
    }

    public void ClearUILines()
    {
        for (int i = 0; i < 7; i++)
        {
            Console.WriteLine();
        }
    }

    public void SendGui(string msg)
    {
        string temp;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        Output.Add(msg);
    }

    public void Update(List<string>? top, int? cardSelection)
    {
        ClearUILines();
        Console.CursorTop = 0;
        Console.CursorLeft = 0;
        PrintTop(top, cardSelection);
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