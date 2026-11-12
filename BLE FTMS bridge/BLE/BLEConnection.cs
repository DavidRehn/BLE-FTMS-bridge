using BLE_FTMS_bridge.BLE;
using BLE_FTMS_bridge.Trainer;
using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private TrainerDevice trainer;
        private FTMSHandler ftmsHandler;
        private FTMSControlPoint? controlPoint;
        public FTMSControlPoint? ControlPoint => controlPoint;


        public BLEConnection(BLEScanner scanner, TrainerDevice trainer)
        {
            device = null;
            this.scanner = scanner;
            ftmsService = null;
            this.trainer = trainer;
            ftmsHandler = new FTMSHandler(trainer);
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

            if (!await ftmsHandler.ReadFitnessMachineFeatures(ftmsService!))
                return false;

            if (!await SubscribeBikeData(ftmsService!))
                return false;

            await SubscribeFitnessMachineStatus(ftmsService!);

            if (!await SubscribeControlPoint(ftmsService!))
                return false;

            controlPoint = new FTMSControlPoint(controlPointCharacteristic);

            controlPoint = new FTMSControlPoint(controlPointCharacteristic!);

            // Request control of the trainer
            var response = await controlPoint.RequestControlAsync();

            if (!response.IsSuccess)
            {
                Debug.WriteLine(
                    $"Failed to obtain control permission: {response.ResultCode}");

                return false;
            }

            return true;
        }




        // ----------------------------------------- Subscribe to characteristic -----------------------------------------


        // Subscribes to the FTMS Indoor Bike Data characteristic
        private async Task<bool> SubscribeBikeData(GattDeviceService ftmsService)
        {
            var ftmsIBDUuid = Guid.Parse("00002AD2-0000-1000-8000-00805f9b34fb");
            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsIBDUuid);

            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0)
                return false;   // Indoor Bike Data characteristic not found

            indoorBikeDataCharacteristic = result.Characteristics.First();

            if (!indoorBikeDataCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                return false;   // Notifications not supported

            //Subscribe to characteristic
            var status = await indoorBikeDataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            if (status != GattCommunicationStatus.Success)
                return false;

            indoorBikeDataCharacteristic.ValueChanged += ftmsHandler.OnBikeDataReceived;

            return true;
        }



        // Subscribes to the FTMS Fitness Machine Status characteristic
        private async Task<bool> SubscribeFitnessMachineStatus(GattDeviceService ftmsService)
        {
            var ftmsStatusUuid = Guid.Parse("00002ADA-0000-1000-8000-00805f9b34fb"); // Fitness Machine Status UUID
            var result = await ftmsService.GetCharacteristicsForUuidAsync(ftmsStatusUuid);

            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0) 
                return false;       // Fitness Machine Status characteristic not found

            fitnessMachineStatusCharacteristic = result.Characteristics.First();

            if (!fitnessMachineStatusCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) 
                return false;   // Notifications not supported

            // Subscribe to characteristic
            var status = await fitnessMachineStatusCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            if (status != GattCommunicationStatus.Success)
                return false;

            fitnessMachineStatusCharacteristic.ValueChanged += ftmsHandler.OnFitnessMachineStatusReceived;  // Handle incoming status data

            return true;
        }



        // Subscribes to FTMS Control point
        private async Task<bool> SubscribeControlPoint(GattDeviceService ftmsService)
        {
            var controlPointUuid = Guid.Parse("00002AD9-0000-1000-8000-00805f9b34fb");
            var result = await ftmsService.GetCharacteristicsForUuidAsync(controlPointUuid);

            if (result.Status != GattCommunicationStatus.Success || result.Characteristics.Count == 0)
                return false;

            controlPointCharacteristic = result.Characteristics.First();

            var properties = controlPointCharacteristic.CharacteristicProperties;

            if (!properties.HasFlag(GattCharacteristicProperties.Indicate))
                return false;

            if (!properties.HasFlag(GattCharacteristicProperties.Write))
                return false;

            // Subscribe to indications
            var status = await controlPointCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);

            return status == GattCommunicationStatus.Success;
        }



        // ----------------------------------------- Handlers for incoming data -----------------------------------------


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
