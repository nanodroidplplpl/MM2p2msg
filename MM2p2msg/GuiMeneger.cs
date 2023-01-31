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
        UpdateGui = updateGui;
        this.usrName = usrName;
        this.kUser = kUser;
        EnableInput = enableInput;
    }

    private User kUser;

    private static readonly string[] SpecialKeys = {@"/changeTab", @"/newConf", @"/deleteConf", @"/addFriend", @"/deleteFriend"};
    
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
    }

    public void UpdateConf()
    {
        
        return;
    }

    public void DeleteConf()
    {
        _top.RemoveAt(cardSelection);
        cardSelection--;
        PrintUi();
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

    public void PrintUi()
    {
        while (true)
        {
            UpdateGui.WaitOne();
            EnableInput.Reset();
            if (cardSelection == 0)
            {
                Object friends = kUser.TryFriendlyContacts(
                    result => 
                        _guis[0].PrintContacts(result, _top, cardSelection));
            }
            else
            {
                _guis[cardSelection].Output = FindCorrectPerson((List<Contacts>)MonitorServerGui.GetMonitoredVar());
                _guis[cardSelection].Update(_top, cardSelection);
            }

            EnableInput.Set();
            UpdateGui.Reset();
            //await Task.Delay(0);
        }
    }

    public int FindSpecialKey(string msg)
    {
        int iter = 0;
        foreach (string key in SpecialKeys)
        {
            Match match = Regex.Match(msg, key);
            if (match.Success)
                return iter;
            iter++;
        }
        return -1;
    }
    public bool CheckForSpecialKeys(string msg)
    {
        switch (FindSpecialKey(msg))
        {
            case 0:
                cardSelection = (cardSelection + 1 > _top.Count - 1) ? 0 : cardSelection + 1;
                //PrintUi();
                UpdateGui.Set();
                return true;
            case 1:
                AddNewConf(msg);
                return true;
            case 2:
                DeleteConf();
                return true;
            case 3:
                break;
        }

        return false;
    }
    
    public void GetUserInput()
    {
        while (true)
        {
            EnableInput.WaitOne();
            //Console.CursorTop += 5;
            Console.CursorLeft = 40;
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
            msg = "Ty: " + msg;
            if (!CheckForSpecialKeys(msg) && cardSelection != 0)
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
                            friend.Conf.Add("[HOST NIE AKTYWNY]");
                    }
                }
                //_guis[cardSelection].Update(_top, cardSelection);
                UpdateGui.Set();
            }
            else if (cardSelection == 0)
            {
                EnableInput.Set();
                _guis[cardSelection].Update(_top, cardSelection);
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            //await Task.Delay(0);
        }
    }
}