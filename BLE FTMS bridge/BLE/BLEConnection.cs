using BLE_FTMS_bridge.Trainer;
using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BLETestApp.BLE
{
    public class BLEConnection
    {
        private BluetoothLEDevice? device;
        private BLEScanner scanner;
        private GattDeviceService? ftmsService;
        //private GattDeviceService? cpsService;    // Not finished implementing 
        private BLEParser parser;
        private TrainerStatus trainer;
        

        public BLEConnection(BLEScanner scanner, TrainerStatus trainer)
        {
            device = null;
            this.scanner = scanner;
            ftmsService = null;
            //cpsService = null;
            this.trainer = trainer;
            parser = new BLEParser();
        }

        // Connects to a scanned ble device (peripheral) with the given address
        public async Task<bool> ConnectDevice(ulong address)
        {
            try
            {
                device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
                if (device == null)
                {
                    scanner.RemoveOldAdvertisements();
                    return false;
                }
                device.ConnectionStatusChanged += Device_ConnectionStatusChanged;
                return await ConnectionSetup();
            }
            catch (Exception e)
            {
                return false;
            }
        }

        // Gets FTMS and CPS Services if supported by the device, otherwise null (device is incompatible)
        private async Task<bool> GetServices()
        {
            var gattServices = await device!.GetGattServicesAsync();
            if (gattServices.Status != GattCommunicationStatus.Success)
            {
                ftmsService = null;
                return false;
            }

            Guid ftmsUuid = Guid.Parse("00001826-0000-1000-8000-00805f9b34fb"); // FTMS
            //Guid cpsUuid = Guid.Parse("00001818-0000-1000-8000-00805f9b34fb"); // CPS
            ftmsService = gattServices.Services.FirstOrDefault(s => s.Uuid == ftmsUuid);
            //GattDeviceService cpsService = gattServices.Services.FirstOrDefault(s => s.Uuid == cpsUuid);
            return ftmsService != null;
        }

        // Subscribes to the FTMS Indoor Bike Data characteristic
        private async Task<bool> SubscribeBikeData(GattDeviceService ftmsService, Action<byte[]> onDataReceived)
        {
            var ftmsIBDUuid = Guid.Parse("00002AD2-0000-1000-8000-00805f9b34fb");   // FTMS Indoor Bike Data UUID
            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsIBDUuid);
            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0)
            {
                // Indoor Bike Data characteristic not found
                return false;
            }

            var characteristic = result.Characteristics.First();
            // Check if notifications are supported
            if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                return false;
            }

            // Handle incoming data
            characteristic.ValueChanged += (sender, args) =>
            {
                var reader = DataReader.FromBuffer(args.CharacteristicValue);

                byte[] data = new byte[args.CharacteristicValue.Length];
                reader.ReadBytes(data);

                onDataReceived?.Invoke(data);
            };

            //Subscribe to characteristic
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.Notify);
            return status == GattCommunicationStatus.Success;
        }

        // Called when data from FTMS Indoor Bike Data is received
        private void OnBikeDataReceived(byte[] data)
        {
            parser.ParseIndoorBikeData(data, trainer.trainerState);
        }

        private async Task<bool> ConnectionSetup()
        {
            if (!await GetServices())
                return false;
            return await SubscribeBikeData(ftmsService!, OnBikeDataReceived);
        }

        private void Device_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            trainer.trainerState.IsConnected = sender.ConnectionStatus == BluetoothConnectionStatus.Connected;
            if (!trainer.trainerState.IsConnected)
                scanner.StartScanning();
            else
                scanner.StopScanning();
        }
    }
}
