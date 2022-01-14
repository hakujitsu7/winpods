using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace WinPods.ViewModels
{
    public class BatteryViewModel : ObservableObject
    {
        private Uri _leftImage;
        public Uri LeftImage
        {
            get => _leftImage;
            set
            {
                if (value != _leftImage)
                {
                    SetProperty(ref _leftImage, value);
                }
            }
        }

        private Uri _rightImage;
        public Uri RightImage
        {
            get => _rightImage;
            set
            {
                if (value != _rightImage)
                {
                    SetProperty(ref _rightImage, value);
                }
            }
        }

        private Uri _caseImage;
        public Uri CaseImage
        {
            get => _caseImage;
            set
            {
                if (value != _caseImage)
                {
                    SetProperty(ref _caseImage, value);
                }
            }
        }

        private string _leftIcon;
        public string LeftIcon
        {
            get => _leftIcon;
            set
            {
                if (value != _leftIcon)
                {
                    SetProperty(ref _leftIcon, value);
                }
            }
        }

        private string _rightIcon;
        public string RightIcon
        {
            get => _rightIcon;
            set
            {
                if (value != _rightIcon)
                {
                    SetProperty(ref _rightIcon, value);
                }
            }
        }

        private string _caseIcon;
        public string CaseIcon
        {
            get => _caseIcon;
            set
            {
                if (value != _caseIcon)
                {
                    SetProperty(ref _caseIcon, value);
                }
            }
        }

        private string _leftBatteryLevel;
        public string LeftBatteryLevel
        {
            get => _leftBatteryLevel;
            set
            {
                if (value != _leftBatteryLevel)
                {
                    SetProperty(ref _leftBatteryLevel, value);
                }
            }
        }

        private string _rightBatteryLevel;
        public string RightBatteryLevel
        {
            get => _rightBatteryLevel;
            set
            {
                if (value != _rightBatteryLevel)
                {
                    SetProperty(ref _rightBatteryLevel, value);
                }
            }
        }

        private string _caseBatteryLevel;
        public string CaseBatteryLevel
        {
            get => _caseBatteryLevel;
            set
            {
                if (value != _caseBatteryLevel)
                {
                    SetProperty(ref _caseBatteryLevel, value);
                }
            }
        }

        public BatteryViewModel()
        {
            _leftImage = new Uri("ms-appx:///Assets/AirPods/Left.png");
            _rightImage = new Uri("ms-appx:///Assets/AirPods/Right.png");
            _caseImage = new Uri("ms-appx:///Assets/AirPods/Case.png");

            _leftIcon = "\xE996";
            _rightIcon = "\xE996";
            _caseIcon = "\xE996";

            _leftBatteryLevel = "Unknown";
            _rightBatteryLevel = "Unknown";
            _caseBatteryLevel = "Unknown";

            (Application.Current as App).BluetoothLEDataUpdated += OnBluetoothLEDataUpdated;
        }

        private async void OnBluetoothLEDataUpdated(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                int[] newBluetoothLEData = (sender as App).BluetoothLEData;

                string[] models = new string[] { "AirPods", "AirPods_Pro" };
                string[,] icons = new string[,] {
                    { "\xE850", "\xE851", "\xE852", "\xE853", "\xE854", "\xE855", "\xE856", "\xE857", "\xE858", "\xE859", "\xE83F" },
                    { "\xE85A", "\xE85B", "\xE85C", "\xE85D", "\xE85E", "\xE85F", "\xE860", "\xE861", "\xE862", "\xE83E", "\xEA93" }
                };

                LeftImage = new Uri($"ms-appx:///Assets/{models[newBluetoothLEData[(int)DataIndex.IsAirPodsPro]]}/Left.png");
                RightImage = new Uri($"ms-appx:///Assets/{models[newBluetoothLEData[(int)DataIndex.IsAirPodsPro]]}/Right.png");
                CaseImage = new Uri($"ms-appx:///Assets/{models[newBluetoothLEData[(int)DataIndex.IsAirPodsPro]]}/Case.png");

                if (newBluetoothLEData[(int)DataIndex.LeftBatteryLevel] >= 0)
                {
                    LeftIcon = icons[
                        newBluetoothLEData[(int)DataIndex.IsChargingLeft],
                        newBluetoothLEData[(int)DataIndex.LeftBatteryLevel] / 10
                    ];
                    LeftBatteryLevel = $"{newBluetoothLEData[(int)DataIndex.LeftBatteryLevel]}%";
                }
                else
                {
                    LeftIcon = "\xE996";
                    LeftBatteryLevel = "Unknown";
                }

                if (newBluetoothLEData[(int)DataIndex.RightBatteryLevel] >= 0)
                {
                    RightIcon = icons[
                        newBluetoothLEData[(int)DataIndex.IsChargingRight],
                        newBluetoothLEData[(int)DataIndex.RightBatteryLevel] / 10
                    ];
                    RightBatteryLevel = $"{newBluetoothLEData[(int)DataIndex.RightBatteryLevel]}%";
                }
                else
                {
                    RightIcon = "\xE996";
                    RightBatteryLevel = "Unknown";
                }

                if (newBluetoothLEData[(int)DataIndex.CaseBatteryLevel] >= 0)
                {
                    CaseIcon = icons[
                        newBluetoothLEData[(int)DataIndex.IsChargingCase],
                        newBluetoothLEData[(int)DataIndex.CaseBatteryLevel] / 10
                    ];
                    CaseBatteryLevel = $"{newBluetoothLEData[(int)DataIndex.CaseBatteryLevel]}%";
                }
                else
                {
                    CaseIcon = "\xE996";
                    CaseBatteryLevel = "Unknown";
                }
            });
        }
    }
}
