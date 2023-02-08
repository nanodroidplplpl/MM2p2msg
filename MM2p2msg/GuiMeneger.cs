using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MM2p2msg;

public class GuiMeneger
{
    public GuiMeneger(string name, AutoResetEvent updateGui, AutoResetEvent enableInput, string usrName, User kUser, int myPort)
    {
        cardSelection = 0;
        _guis = new List<UserInterface>();
        _top = new List<string>();
        Name = name;
        _guis.Add(new UserInterface(new Contacts("Menu", "helo", 0, false, 0, null, null)));
        _top.Add("Menu");
        _guis[0].Output.Add(".NET chat, Nowy Elegancki Terminal Net");
        _guis[0].Output.Add("Znajomi");
        UpdateGui = updateGui;
        this.usrName = usrName;
        this.kUser = kUser;
        EnableInput = enableInput;
        this.myPort = myPort;
    }

    private User kUser;

    private int myPort;
    
    public enum SpecialKey
    {
        ChangeTab,
        NewConf,
        DeleteConf,
        AddFriend,
        DeleteFriend,
        Exit,
        Wrong
    }

    private static readonly string[] SpecialKeys = {@"/changeTab", @"/newConf",
        @"/deleteConf", @"/addFriend", @"/removeFriend", @"/exit"};
    
    public List<UserInterface> _guis;

    public int cardSelection;

    public List<string> _top;

    public string Name;

    public AutoResetEvent UpdateGui;
    
    public AutoResetEvent EnableInput;

    public string usrName;
    
    public MonitorServerGui MonitorServerGui { get; set; }
    
    public void AddNewConf(string msg)
    {
        Match match = Regex.Match(msg, @"/newConf (\w+) (\d+\.\d+\.\d+\.\d+) (\d+)");
        List<Contacts> friends = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
        foreach (var friend in friends)
        {
            if (match.Groups[1].Value == friend.Name && match.Groups[2].Value == friend.Ip && friend.Active && int.Parse(match.Groups[3].Value) == friend.Port)
            {
                UserInterface ui = new UserInterface(friend);
                _guis.Add(ui);
                _top.Add(ui.Name);
                cardSelection = _top.Count - 1;
                UpdateGui.Set();
                return;
            }
        }

        EnableInput.Set();
    }

    public bool CheckIpClones(List<Contacts> con, string ip)
    {
        foreach (var c in con)
        {
            if (c.Ip == ip)
            {
                return true;
            }
        }
        return false;
    }
    
    public void RemoveContact(List<Contacts> con, string ip, int port)
    {
        int jter = 0;
        foreach (var c in con)
        {
            if (c.Ip == ip && c.Port == port)
            {

                int iter = 0;
                foreach (var gui in _guis)
                {
                    if (gui.ip == ip)
                    {
                        cardSelection = iter;
                        DeleteConf();
                    }
                    iter++;
                }
                break;
            }
            jter++;
        }
        con.RemoveAt(jter);
        MonitorServerGui.SetMonitoredVar(con);
        _guis[0].Output.Clear();
        _guis[0].Output.Add(".NET chat, Nowy Elegancki Terminal Net");
        _guis[0].Output.Add("Znajomi");
        UpdateGui.Set();
    }

    public void DeleteFriend(string msg)
    {
        string pattern = @"/removeFriend\s+(\w+)\s+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+(\d+)";
        Match match = Regex.Match(msg, pattern);

        if (match.Success)
        {
            string name = match.Groups[1].Value;
            string ipAddress = match.Groups[2].Value;
            int conPort = int.Parse(match.Groups[3].Value);
            List<Contacts> con = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
            RemoveContact(con, ipAddress, conPort);
        }
    }

    public async void AddFriend(string msg)
    {
        EnableInput.Reset();
        List<Contacts> con = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
        //string pattern = @"/addFriend\s+(\w+)\s+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})";
        string pattern = @"/addFriend\s+(\w+)\s+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+(\d+)";
        Match match = Regex.Match(msg, pattern);
        string FriendNick = match.Groups[1].Value;
        string? friendIp = match.Groups[2].Value;
        int port = int.Parse(match.Groups[3].Value);
        Match s = Regex.Match(friendIp,@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
        if (!CheckIpClones(con, friendIp) && s.Success)
        {
            Contacts c = new Contacts(FriendNick, friendIp, port, false, 660, null, null);
            c.C = new Client(c, usrName);
            c = await c.C.TryConnect(myPort);
            con.Add(c);
            kUser.SaveContactToJson(FriendNick, friendIp, port);
            MonitorServerGui.SetMonitoredVar(con);
        }
        Console.Clear();
        UpdateGui.Set();
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }

    public void DeleteConf()
    {
        _top.RemoveAt(cardSelection);
        cardSelection--;
        UpdateGui.Set();
    }

    public List<string> FindCorrectPerson(List<Contacts> con)
    {
        foreach (var v in con)
        {
            if (v.Ip == _guis[cardSelection].ip && v.Port == _guis[cardSelection].port)
            {
                return v.Conf;
            }
        }

        return null;
    }

    public void PrintUi(CancellationToken endProgram)
    {
        while (!endProgram.IsCancellationRequested)
        {
            UpdateGui.WaitOne();
            EnableInput.Reset();
            if (cardSelection == 0)
            {
                Console.Clear();
                _guis[0].Output.Clear();
                List<Contacts> friends = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                foreach (var friend in friends)
                {
                    _guis[0].PrintContacts(friend, _top, cardSelection);
                }
                Console.CursorTop = 8;
                Console.CursorLeft = 20;
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
                Console.CursorTop = 8;
            }
            else
            {
                Console.Clear();
                _guis[cardSelection].Output = FindCorrectPerson((List<Contacts>)MonitorServerGui.GetMonitoredVar());
                _guis[cardSelection].Update(_top, cardSelection);
            }
            
            UpdateGui.Reset();
            EnableInput.Set();
        }
        Console.Clear();
        Console.WriteLine("Koniec");
    }

    public SpecialKey FindSpecialKey(string msg)
    {
        int iter = 0;
        foreach (string key in SpecialKeys)
        {
            Match match = Regex.Match(msg, key);
            if (match.Success)
                return (SpecialKey) iter;
            iter++;
        }
        return SpecialKey.Wrong;
    }
    public bool CheckForSpecialKeys(string msg, CancellationTokenSource endProgram)
    {
        _guis[0].Output.Clear();
        switch (FindSpecialKey(msg))
        {
            case SpecialKey.ChangeTab:
                cardSelection = (cardSelection + 1 > _top.Count - 1) ? 0 : cardSelection + 1;
                UpdateGui.Set();
                return true;
            case SpecialKey.NewConf:
                AddNewConf(msg);
                return true;
            case SpecialKey.DeleteConf:
                if (cardSelection != 0) DeleteConf();
                else EnableInput.Set();
                return true;
            case SpecialKey.AddFriend:
                AddFriend(msg);
                return true;
            case SpecialKey.DeleteFriend:
                DeleteFriend(msg);
                EnableInput.Set();
                return true;
            case SpecialKey.Exit:
                endProgram.Cancel(); 
                List<Contacts> kontakty = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                kUser.SaveConf(kontakty);
                UpdateGui.Set();
                EnableInput.Set();
                List<Contacts> kontakt = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                kUser.SaveConotactToJsonOnExit(kontakt);
                return true;
                break;
        }

        return false;
    }
    
    public void GetUserInput(CancellationToken _endProgram, CancellationTokenSource endProgram)
    {
        while (true)
        {
            if (_endProgram.IsCancellationRequested)
            {
                Console.WriteLine("Koniec Watku menegeraGui");
                return;
            }
            EnableInput.WaitOne();
            Console.CursorTop = 8;
            Console.CursorLeft = 20;
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
            Debug.WriteLine(msg);
            Console.CursorTop -= 1;
            if (msg != null)
                for (int i = 0; i < msg.Length; i++)
                {
                    Console.CursorLeft = i;
                    Console.Write(" ");
                }

            Console.CursorLeft = 0;
            if (!CheckForSpecialKeys(msg, endProgram))
            {
                if (cardSelection != 0)
                {
                    List<Contacts> temp = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                    foreach (var friend in temp)
                    {
                        if (friend.Name == _guis[cardSelection].Name && friend.Ip == _guis[cardSelection].ip)
                        {
                            Client client = new Client(friend, usrName);
                            friend.Active = client.SendMessage(usrName+": "+msg);
                            if (friend.Active)
                                friend.Conf.Add(usrName+": "+msg);
                            else
                                friend.Conf.Add("[HOST NIEAKTYWNY]");
                        }
                    }
                    EnableInput.Reset();
                    UpdateGui.Set();
                }
                else if (cardSelection == 0)
                {
                    EnableInput.Set();
                    _guis[cardSelection].Update(_top, cardSelection);
                }
            }
            msg = "";
        }
    }
}