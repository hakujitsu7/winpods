using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

using WinPods.Services;

namespace WinPods
{
    public enum DataIndex
    {
        IsAirPodsPro,
        LeftBatteryLevel,
        RightBatteryLevel,
        CaseBatteryLevel,
        IsChargingLeft,
        IsChargingRight,
        IsChargingCase,
        InEarLeft,
        InEarRight
    }

    public sealed partial class App : Application
    {
        public int[] BluetoothLEData { get; private set; }
        public event EventHandler BluetoothLEDataUpdated;

        private Lazy<ActivationService> _activationService;

        private BackgroundTaskDeferral _backgroundTaskDeferral = null;
        private AppServiceConnection _appServiceconnection = null;

        private bool _isForeground = false;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            EnteredBackground += delegate { _isForeground = false; };
            LeavingBackground += delegate { _isForeground = true; };

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.BatteryPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
            {
                _backgroundTaskDeferral = args.TaskInstance.GetDeferral();

                _appServiceconnection = details.AppServiceConnection;
                _appServiceconnection.RequestReceived += OnRequestReceived;

                args.TaskInstance.Canceled += OnTaskCanceled;
            }
        }

        private void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDeferral = args.GetDeferral();

            ValueSet message = args.Request.Message;

            string command = message["Command"] as string;
            object parameter = message["Parameter"];

            switch (command)
            {
                case "UpdateBluetoothLEData":
                    if (_isForeground)
                    {
                        BluetoothLEData = parameter as int[];
                        OnBluetoothLEDataUpdated();
                    }
                    break;
                case "ExitApp":
                    Current.Exit();
                    break;
            }

            messageDeferral.Complete();
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_backgroundTaskDeferral != null)
            {
                _backgroundTaskDeferral.Complete();
            }
        }

        private void OnBluetoothLEDataUpdated()
        {
            BluetoothLEDataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
