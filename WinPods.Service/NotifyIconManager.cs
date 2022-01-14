using System.Linq;
using System.Windows.Forms;
using WinPods.Service.Notify;

namespace WinPods.Service
{
    public sealed class NotifyIconManager : Notifier
    {
        private readonly NotifyIcon _notifyIcon = null;

        public NotifyIconManager()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.Icon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Open", delegate { OnNotified("OpenClicked"); }),
                    new MenuItem("Exit", delegate { OnNotified("ExitClicked"); }),
                }),
                Text = "Searching...",
                Visible = true,
            };
            _notifyIcon.DoubleClick += delegate { OnNotified("DoubleClicked"); };
        }

        public void UpdateNotifyIcon(int[] bluetoothLEData)
        {
            string[] texts = { "", "", "" };

            if (bluetoothLEData[(int)DataIndex.LeftBatteryLevel] >= 0)
            {
                texts[0] = $"Left {bluetoothLEData[(int)DataIndex.LeftBatteryLevel]}%";
                if (bluetoothLEData[(int)DataIndex.IsChargingLeft] == 1)
                {
                    texts[0] += " (Charging)";
                }
            }

            if (bluetoothLEData[(int)DataIndex.RightBatteryLevel] >= 0)
            {
                texts[1] = $"Right {bluetoothLEData[(int)DataIndex.RightBatteryLevel]}%";
                if (bluetoothLEData[(int)DataIndex.IsChargingRight] == 1)
                {
                    texts[1] += " (Charging)";
                }
            }

            if (bluetoothLEData[(int)DataIndex.CaseBatteryLevel] >= 0)
            {
                texts[2] = $"Case {bluetoothLEData[(int)DataIndex.CaseBatteryLevel]}%";
                if (bluetoothLEData[(int)DataIndex.IsChargingCase] == 1)
                {
                    texts[2] += " (Charging)";
                }
            }

            _notifyIcon.Text = string.Join("\n", texts.Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }
    }
}
