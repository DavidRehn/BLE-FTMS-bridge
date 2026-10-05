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
        private GattCharacteristic? indoorBikeDataCharacteristic;
        private GattCharacteristic? fitnessMachineStatusCharacteristic;
        private GattCharacteristic? controlPointCharacteristic;
        private BLEParser parser;
        private TrainerDevice trainer;
        

        public BLEConnection(BLEScanner scanner, TrainerDevice trainer)
        {
            device = null;
            this.scanner = scanner;
            ftmsService = null;
            this.trainer = trainer;
            parser = new BLEParser();
        }


        // ----------------------------------------- Connection setup -----------------------------------------

        // Connects to a scanned ble device with the given address
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

        // Gets FTMS service if supported by the device, otherwise null (device is incompatible)
        private async Task<bool> GetServices()
        {
            var gattServices = await device!.GetGattServicesAsync();
            if (gattServices.Status != GattCommunicationStatus.Success)
            {
                ftmsService = null;
                return false;
            }

            Guid ftmsUuid = Guid.Parse("00001826-0000-1000-8000-00805f9b34fb"); 
            ftmsService = gattServices.Services.FirstOrDefault(s => s.Uuid == ftmsUuid); 
            return ftmsService != null;
        }


        private async Task<bool> ConnectionSetup()
        {
            if (!await GetServices())
                return false;

            if (!await ReadFitnessMachineFeatures(ftmsService!))
                return false;

            if (!await SubscribeBikeData(ftmsService!, OnBikeDataReceived))
                return false;

            await SubscribeFitnessMachineStatus(ftmsService!, OnFitnessMachineStatusReceived);  // Status is optional

            return true;
        }




        // ----------------------------------------- Subscribe to characteristic -----------------------------------------


        // Subscribes to the FTMS Indoor Bike Data characteristic
        private async Task<bool> SubscribeBikeData(GattDeviceService ftmsService, Action<byte[]> onDataReceived)
        {
            var ftmsIBDUuid = Guid.Parse("00002AD2-0000-1000-8000-00805f9b34fb");
            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsIBDUuid);
            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0)
            {
                // Indoor Bike Data characteristic not found
                return false;
            }

            indoorBikeDataCharacteristic = result.Characteristics.First();

            // Check if notifications are supported
            if (!indoorBikeDataCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                return false;
            }

            // Handle incoming data
            indoorBikeDataCharacteristic.ValueChanged += (sender, args) =>
            {
                var reader = DataReader.FromBuffer(args.CharacteristicValue);

                byte[] data = new byte[args.CharacteristicValue.Length];
                reader.ReadBytes(data);

                onDataReceived?.Invoke(data);
            };

            //Subscribe to characteristic
            var status = await indoorBikeDataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            return status == GattCommunicationStatus.Success;
        }



        // Subscribes to the FTMS Fitness Machine Status characteristic
        private async Task<bool> SubscribeFitnessMachineStatus(GattDeviceService ftmsService, Action<byte[]> onStatusReceived)
        {
            var ftmsStatusUuid = Guid.Parse("00002ADA-0000-1000-8000-00805f9b34fb"); // Fitness Machine Status UUID

            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsStatusUuid);

            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0) 
                return false;       // Fitness Machine Status characteristic not found

            fitnessMachineStatusCharacteristic = result.Characteristics.First();

            // Check if notifications are supported
            if (!fitnessMachineStatusCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) 
                return false;

            // Handle incoming status data
            fitnessMachineStatusCharacteristic.ValueChanged += (sender, args) =>
            {
                var reader = DataReader.FromBuffer(args.CharacteristicValue);

                byte[] data = new byte[args.CharacteristicValue.Length];
                reader.ReadBytes(data);

                onStatusReceived?.Invoke(data);
            };

            // Subscribe to characteristic
            var status = await fitnessMachineStatusCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            return status == GattCommunicationStatus.Success;
        }


        // ----------------------------------------- Handlers for incoming data -----------------------------------------


        // Called when data from FTMS Indoor Bike Data is received
        private void OnBikeDataReceived(byte[] data)
        {
            parser.ParseIndoorBikeData(data, trainer.trainerState);
        }


        private void Device_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            trainer.trainerState.IsConnected = sender.ConnectionStatus == BluetoothConnectionStatus.Connected;
            if (!trainer.trainerState.IsConnected)
                scanner.StartScanning();
            else
                scanner.StopScanning();
        }


        // Called when Fitness Machine Status data is received
        private void OnFitnessMachineStatusReceived(byte[] data)
        {
            // Parse the FTMS status here
        }


        // Reads the FTMS Fitness Machine Feature characteristic
        private async Task<bool> ReadFitnessMachineFeatures(GattDeviceService ftmsService)
        {
            var ftmsFeatureUuid = Guid.Parse("00002ACC-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsFeatureUuid);

            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseFitnessMachineFeatures(data, trainer.trainerCapabilities);

            return true;
        }


        
    }
}
