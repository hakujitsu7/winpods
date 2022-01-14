using System;

namespace WinPods.Service
{
    public sealed class NotifyEventArgs : EventArgs
    {
        public string Reason { get; }

        public NotifyEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}
