using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;

namespace WinPods.Service
{
    public sealed class MainService : ApplicationContext
    {
        private readonly MessageDispatcher _messageDispatcher = null;
        private readonly BluetoothLEService _bluetoothLEService = null;
        private readonly NotifyIconManager _notifyIconManager = null;

        public MainService()
        {
            _messageDispatcher = new MessageDispatcher();

            _bluetoothLEService = new BluetoothLEService();
            _bluetoothLEService.Notified += OnNotified;

            _notifyIconManager = new NotifyIconManager();
            _notifyIconManager.Notified += OnNotified;
        }

        public async void OnNotified(object sender, NotifyEventArgs e)
        {
            switch (e.Reason)
            {
                case "BluetoothLEDataChanged":
                    int[] newBluetoothLEData = (sender as BluetoothLEService).BluetoothLEData;
                    _notifyIconManager.UpdateNotifyIcon(newBluetoothLEData);
                    _ = await _messageDispatcher.SendMessageAsync(MessageDispatcherPriority.Normal, "UpdateBluetoothLEData", newBluetoothLEData);
                    break;

                case "DoubleClicked":
                case "OpenClicked":
                    IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
                    _ = await appListEntries.First().LaunchAsync();
                    break;

                case "ExitClicked":
                    _ = await _messageDispatcher.SendMessageAsync(MessageDispatcherPriority.High, "ExitApp");
                    Application.Exit();
                    break;
            }
        }
    }
}
