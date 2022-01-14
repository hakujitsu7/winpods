using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using WinPods.ViewModels;

namespace WinPods.Views
{
    public sealed partial class BatteryPage : Page
    {
        public BatteryViewModel ViewModel { get; } = new BatteryViewModel();

        public BatteryPage()
        {
            InitializeComponent();
        }
    }
}
