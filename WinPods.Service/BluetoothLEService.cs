using System.Collections.Generic;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using WinPods.Service.Notify;

namespace WinPods.Service
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

    public sealed class BluetoothLEService : Notifier
    {
        public int[] BluetoothLEData { get; private set; }

        private readonly BluetoothLEAdvertisementWatcher _watcher = null;

        public BluetoothLEService()
        {
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            _watcher.SignalStrengthFilter.InRangeThresholdInDBm = -60;
            _watcher.Received += OnAdvertisementReceived;
            _watcher.Start();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            IReadOnlyList<BluetoothLEManufacturerData> manufacturerDatas = eventArgs.Advertisement.GetManufacturerDataByCompanyId(76);
            foreach (BluetoothLEManufacturerData manufacturerData in manufacturerDatas)
            {
                IBuffer dataBuffer = manufacturerData.Data;
                if (dataBuffer.Length == 27)
                {
                    byte[] rawBluetoothLEData = new byte[27];

                    DataReader dataReader = DataReader.FromBuffer(dataBuffer);
                    dataReader.ReadBytes(rawBluetoothLEData);

                    if (rawBluetoothLEData[0] == 0x07 && rawBluetoothLEData[1] == 0x19)
                    {
                        BluetoothLEData = ParseBluetoothLEData(rawBluetoothLEData);
                        OnNotified("BluetoothLEDataChanged");
                    }
                }
            }
        }

        private int[] ParseBluetoothLEData(byte[] rawData)
        {
            int[] parsedData = new int[]
            {
                ((rawData[3] & 0x0F) >> 0) == 0x0E ? 1 : 0,                 // 에어팟 프로 여부
                (rawData[6] & 0x0F) >> 0,                                   // 왼쪽 배터리 잔량
                (rawData[6] & 0xF0) >> 4,                                   // 오른쪽 배터리 잔량
                (rawData[7] & 0x0F) >> 0,                                   // 케이스 배터리 잔량
                (((rawData[7] & 0xF0) >> 4) & 0b00000001) != 0 ? 1 : 0,     // 왼쪽 충전 여부
                (((rawData[7] & 0xF0) >> 4) & 0b00000010) != 0 ? 1 : 0,     // 오른쪽 충전 여부
                (((rawData[7] & 0xF0) >> 4) & 0b00000100) != 0 ? 1 : 0,     // 케이스 충전 여부
                (((rawData[5] & 0x0F) >> 0) & 0b00000010) != 0 ? 1 : 0,     // 왼쪽 착용 여부
                (((rawData[5] & 0x0F) >> 0) & 0b00001000) != 0 ? 1 : 0,     // 오른쪽 착용 여부
            };

            if ((((rawData[5] & 0xF0) >> 4) & 0b00000010) == 0) // 뒤집혔다면
            {
                Swap(
                    ref parsedData[(int)DataIndex.LeftBatteryLevel],
                    ref parsedData[(int)DataIndex.RightBatteryLevel]
                );
                Swap(
                    ref parsedData[(int)DataIndex.IsChargingLeft],
                    ref parsedData[(int)DataIndex.IsChargingRight]
                );
                Swap(
                    ref parsedData[(int)DataIndex.InEarLeft],
                    ref parsedData[(int)DataIndex.InEarRight]
                );
            }

            for (int i = 1; i <= 3; i++) // 16진수 값에서 백분율 값으로 변환
            {
                if (parsedData[i] == 0x0F)
                {
                    parsedData[i] = -1;
                }
                else
                {
                    parsedData[i] *= 10;
                    if (parsedData[i] != 100)
                    {
                        parsedData[i] += 5;
                    }
                }
            }

            return parsedData;
        }

        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
