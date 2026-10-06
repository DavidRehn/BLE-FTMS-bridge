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


            // Parse the FTMS status here
            Console.WriteLine("test");
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
    }
}
