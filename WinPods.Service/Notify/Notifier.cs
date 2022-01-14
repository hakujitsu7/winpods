namespace WinPods.Service.Notify
{
    public class Notifier : INotify
    {
        public event NotifyEventHandler Notified;

        protected void OnNotified(string reason)
        {
            Notified?.Invoke(this, new NotifyEventArgs(reason));
        }
    }
}
