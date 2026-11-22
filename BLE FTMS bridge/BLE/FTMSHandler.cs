using BLE_FTMS_bridge.BLE.Enum;
using BLE_FTMS_bridge.Trainer;
using BLETestApp.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BLE_FTMS_bridge.BLE
{
    public class FTMSHandler
    {
        private BLEParser parser;
        private TrainerDevice trainer;

        public FTMSHandler(TrainerDevice trainer)
        {
            this.parser = new BLEParser();
            this.trainer = trainer;
        }


        // Called when data from FTMS Indoor Bike Data is received
        public void OnBikeDataReceived(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[args.CharacteristicValue.Length];
            reader.ReadBytes(data);
            parser.ParseIndoorBikeData(data, trainer.trainerState);
        }


        // Called when Fitness Machine Status data is received
        public void OnFitnessMachineStatusReceived(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[args.CharacteristicValue.Length];
            reader.ReadBytes(data);

            Console.WriteLine($"Fitness Machine Status: {BitConverter.ToString(data)}");

            HandleSpinDownStatus(data);

            parser.ParseFitnessMachineStatus(data, trainer.trainerStatus, trainer.trainerConfiguration);
        }


        // Reads the FTMS Fitness Machine Feature characteristic
        public async Task<bool> ReadFitnessMachineFeatures(GattDeviceService ftmsService)
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


        // Reads the Supported Resistance Level Range characteristicz
        public async Task<bool> ReadSupportedResistanceRange(GattDeviceService ftmsService)
        {
            var uuid = Guid.Parse("00002AD6-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(uuid);

            if (result.Status != GattCommunicationStatus.Success ||
                result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseSupportedResistanceRange(data, trainer.trainerCapabilities);

            return true;
        }


        // Reads the Supported Heart Rate Range characteristic
        public async Task<bool> ReadSupportedHeartRateRange(GattDeviceService ftmsService)
        {
            var uuid = Guid.Parse("00002AD7-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(uuid);

            if (result.Status != GattCommunicationStatus.Success ||
                result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseSupportedHeartRateRange(data, trainer.trainerCapabilities);

            return true;
        }



        // Reads the Supported Speed Range characteristic
        public async Task<bool> ReadSupportedSpeedRange(GattDeviceService ftmsService)
        {
            var uuid = Guid.Parse("00002AD4-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(uuid);

            if (result.Status != GattCommunicationStatus.Success ||
                result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseSupportedSpeedRange(data, trainer.trainerCapabilities);

            return true;
        }



        // Reads the Supported Inclination Range characteristic
        public async Task<bool> ReadSupportedInclinationRange(GattDeviceService ftmsService)
        {
            var uuid = Guid.Parse("00002AD5-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(uuid);

            if (result.Status != GattCommunicationStatus.Success ||
                result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseSupportedInclinationRange(data, trainer.trainerCapabilities);

            return true;
        }



        // Reads the Supported Power Range characteristic
        public async Task<bool> ReadSupportedPowerRange(GattDeviceService ftmsService)
        {
            var uuid = Guid.Parse("00002AD8-0000-1000-8000-00805f9b34fb");

            var result = await ftmsService.GetCharacteristicsForUuidAsync(uuid);

            if (result.Status != GattCommunicationStatus.Success ||
                result.Characteristics.Count == 0)
                return false;

            var characteristic = result.Characteristics.First();

            var readResult = await characteristic.ReadValueAsync();

            if (readResult.Status != GattCommunicationStatus.Success)
                return false;

            var reader = DataReader.FromBuffer(readResult.Value);
            byte[] data = new byte[readResult.Value.Length];
            reader.ReadBytes(data);

            parser.ParseSupportedPowerRange(data, trainer.trainerCapabilities);

            return true;
        }


        private void HandleSpinDownStatus(byte[] data)
        {
            if (data.Length < 2 || data[0] != 0x14)
                return;

            Console.WriteLine($"Spin-down notification: {BitConverter.ToString(data)}");

            switch (data[1])
            {
                case 0x01:
                    trainer.trainerStatus.SpinDownStatus = SpinDownStatus.Requested;
                    break;

                case 0x02:
                    trainer.trainerStatus.SpinDownStatus = SpinDownStatus.Success;
                    break;

                case 0x03:
                    trainer.trainerStatus.SpinDownStatus = SpinDownStatus.Error;
                    break;

                case 0x04:
                    trainer.trainerStatus.SpinDownStatus = SpinDownStatus.StopPedaling;
                    break;
            }
        }
    }
}
