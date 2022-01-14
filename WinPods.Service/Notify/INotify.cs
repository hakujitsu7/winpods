namespace WinPods.Service
{
    public delegate void NotifyEventHandler(object sender, NotifyEventArgs e);

    public interface INotify
    {
        event NotifyEventHandler Notified;
    }
}
