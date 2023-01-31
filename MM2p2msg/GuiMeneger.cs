using System.Text.RegularExpressions;

namespace MM2p2msg;

public class GuiMeneger
{
    public GuiMeneger(string name, AutoResetEvent updateGui, AutoResetEvent enableInput, string usrName, User kUser)
    {
        cardSelection = 0;
        _guis = new List<UserInterface>();
        //_menu = new ControlUI(name);
        _top = new List<string>();
        Name = name;
        _guis.Add(new UserInterface(new Contacts("Menu", "helo", 124, false, 111, null, null)));
        _top.Add("Menu");
        _guis[0].Output.Add(".NET chat, Nowy Elegancki Terminal Net");
        _guis[0].Output.Add("Znajomi");
        UpdateGui = updateGui;
        this.usrName = usrName;
        this.kUser = kUser;
        EnableInput = enableInput;
    }

    private User kUser;
    
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
        @"/deleteConf", @"/addFriend", @"/deleteFriend", @"/exit"};
    
    public List<UserInterface> _guis;

    //private ControlUI? _menu;

    public int cardSelection;

    public List<string> _top;

    public string Name;

    public AutoResetEvent UpdateGui;
    
    public AutoResetEvent EnableInput;

    public string usrName;
    
    public MonitorServerGui MonitorServerGui { get; set; }
    
    public void AddNewConf(string msg)
    {
        Match match = Regex.Match(msg, @"/newConf (\w+) (\d+\.\d+\.\d+\.\d+)");
        List<Contacts> friends = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
        foreach (var friend in friends)
        {
            if (match.Groups[1].Value == friend.Name && match.Groups[2].Value == friend.Ip && friend.Active)
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

    public void UpdateConf()
    {
        
        return;
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
            if (v.Ip == _guis[cardSelection].ip)
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
                // Object friends = kUser.TryFriendlyContacts(
                //     result => 
                //         _guis[0].PrintContacts(result, _top, cardSelection));
                Console.Clear();
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
            //await Task.Delay(0);
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
        switch (FindSpecialKey(msg))
        {
            case SpecialKey.ChangeTab:
                cardSelection = (cardSelection + 1 > _top.Count - 1) ? 0 : cardSelection + 1;
                //PrintUi();
                UpdateGui.Set();
                return true;
            case SpecialKey.NewConf:
                AddNewConf(msg);
                return true;
            case SpecialKey.DeleteConf:
                DeleteConf();
                return true;
            case SpecialKey.Exit:
                endProgram.Cancel(); 
                List<Contacts> kontakty = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                kUser.SaveConf(kontakty);
                UpdateGui.Set();
                EnableInput.Set();
                // Console.Clear();
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
            //Console.CursorTop += 5;
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
                    //_guis[cardSelection].SendGui(msg);
                    //_guis[cardSelection].Output.Add(msg);
                    List<Contacts> temp = (List<Contacts>)MonitorServerGui.GetMonitoredVar();
                    foreach (var friend in temp)
                    {
                        if (friend.Name == _guis[cardSelection].Name && friend.Ip == _guis[cardSelection].ip)
                        {
                            Client client = new Client(friend, usrName);
                            friend.Active = client.SendMessage(msg);
                            if (friend.Active)
                                friend.Conf.Add(msg);
                            else
                                friend.Conf.Add("[HOST NIEAKTYWNY]");
                        }
                    }
                    //_guis[cardSelection].Update(_top, cardSelection);
                    //Debug.WriteLine("Dupa 1");
                    EnableInput.Reset();
                    UpdateGui.Set();
                }
                else if (cardSelection == 0)
                {
                    EnableInput.Set();
                    _guis[cardSelection].Update(_top, cardSelection);
                }
            }
            // Console.ForegroundColor = ConsoleColor.White;
            // Console.BackgroundColor = ConsoleColor.Black;
            // EnableInput.Reset();
            //await Task.Delay(0);
            msg = "";
        }
    }
}