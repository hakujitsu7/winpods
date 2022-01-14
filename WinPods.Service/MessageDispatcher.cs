using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WinPods.Service
{
    public enum MessageDispatcherPriority
    {
        High = -1,
        Normal = 0,
        Low = 1,
        Idle = 2
    }

    public sealed class MessageDispatcher
    {
        private struct QueueItem
        {
            public ValueSet Message;
            public TaskCompletionSource<AppServiceResponse> TaskSource;
        }

        private readonly ConcurrentPriorityQueue<int, QueueItem> _messagePriorityQueue = null;

        private AppServiceConnection _appServiceConnection = null;

        public MessageDispatcher()
        {
            _messagePriorityQueue = new ConcurrentPriorityQueue<int, QueueItem>();

            new Thread(Worker)
            {
                IsBackground = true
            }.Start();
        }

        public Task<AppServiceResponse> SendMessageAsync(MessageDispatcherPriority priority, string command, object parameter = null)
        {
            QueueItem item = new QueueItem
            {
                Message = new ValueSet
                {
                    { "Command", command },
                    { "Parameter", parameter }
                },
                TaskSource = new TaskCompletionSource<AppServiceResponse>()
            };
            _messagePriorityQueue.Enqueue((int)priority, item);

            return item.TaskSource.Task;
        }

        private async void Worker()
        {
            while (true)
            {
                if (_messagePriorityQueue.TryDequeue(out KeyValuePair<int, QueueItem> keyValuePair))
                {
                    if (_appServiceConnection == null)
                    {
                        _appServiceConnection = new AppServiceConnection
                        {
                            AppServiceName = "com.hakujitsu.winpods",
                            PackageFamilyName = Package.Current.Id.FamilyName
                        };
                        _appServiceConnection.ServiceClosed += delegate { _appServiceConnection = null; };

                        AppServiceConnectionStatus status = await _appServiceConnection.OpenAsync();
                        if (status != AppServiceConnectionStatus.Success)
                        {
                            _appServiceConnection = null;
                            keyValuePair.Value.TaskSource.SetResult(null);
                        }
                    }

                    keyValuePair.Value.TaskSource.SetResult(
                        await _appServiceConnection.SendMessageAsync(keyValuePair.Value.Message)
                    );
                }

                await Task.Delay(1);
            }
        }
    }
}
