namespace MM2p2msg;

public class MonitorServerGui : Monitor<List<Contacts>>
{
    public MonitorServerGui(List<Contacts> monitoredVar) : base(monitoredVar) {}

    public override object GetMonitoredVar()
    {
        DataAcces.WaitOne();
        try
        {
            return MonitoredVar.ToList();
        }
        finally
        {
            DataAcces.ReleaseMutex();
        }
    }

    public override object SetMonitoredVar(List<Contacts> changeValue)
    {
        DataAcces.WaitOne();
        try
        {
            MonitoredVar = changeValue;
        }
        finally
        {
            DataAcces.ReleaseMutex();
        }

        return true;
    }

    // public new void Dispose()
    // {
    //     DataAcces.Dispose();
    //     base.Dispose();
    // }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            DataAcces.Dispose();
        }

        base.Dispose(disposing);

        _disposed = true;
    }
}