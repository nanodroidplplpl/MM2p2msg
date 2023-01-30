namespace MM2p2msg;

public abstract class Monitor<T> : IDisposable
{
    protected Monitor(T monitoredVar)
    {
        MonitoredVar = monitoredVar;
        DataAcces = new Mutex();
    }

    protected T MonitoredVar { get; set; }
    protected Mutex DataAcces { get; }
    
    public abstract object GetMonitoredVar();
    public abstract object SetMonitoredVar(T changeValue);
    public abstract void Dispose();
}