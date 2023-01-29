namespace MM2p2msg;

public abstract class Monitor : IDisposable
{
    private object _monitoredVar;

    protected Monitor(object monitoredVar)
    {
        _monitoredVar = monitoredVar;
    }

    public abstract object GetMonitoredVar();
    public abstract object SetMonitoredVar();

    public abstract void Dispose();
}