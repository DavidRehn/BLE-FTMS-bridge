using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BLETestApp.BLE
{
    public class BLEParser
    {
        public void ParseIndoorBikeData(byte[] data, TrainerState trainerState)
        {
            double speed = 0;
            double averageSpeed = 0;

            double cadence = 0;
            double averageCadence = 0;

            int power = 0;
            int averagePower = 0;

            double distance = 0;

            double resistanceLevel = 0;

            int totalEnergy = 0;
            int heartRate = 0;

            if (data == null || data.Length < 2)
                return;

            int index = 0;

            // First 2 bytes are flags
            ushort flags = BitConverter.ToUInt16(data, index);
            index += 2;


            // -------------------------------------------------
            // Instantaneous Speed
            // -------------------------------------------------

            bool speedPresent = (flags & 0x0001) == 0;

            if (speedPresent)
            {
                ushort rawSpeed = BitConverter.ToUInt16(data, index);
                speed = rawSpeed / 100.0;
                trainerState.SpeedKph = speed;
                index += 2;
            }

            // -------------------------------------------------
            // Average Speed
            // -------------------------------------------------

            if ((flags & (1 << 1)) != 0)
            {
                ushort rawAverageSpeed = BitConverter.ToUInt16(data, index);
                averageSpeed = rawAverageSpeed / 100.0;
                trainerState.AverageSpeedKph = averageSpeed;
                index += 2;
            }

            // -------------------------------------------------
            // Instantaneous Cadence
            // -------------------------------------------------

            if ((flags & (1 << 2)) != 0)
            {
                ushort rawCadence = BitConverter.ToUInt16(data, index);
                cadence = rawCadence / 2.0;
                trainerState.CadenceRpm = cadence;
                index += 2;
            }

            // -------------------------------------------------
            // Average Cadence
            // -------------------------------------------------

            if ((flags & (1 << 3)) != 0)
            {
                ushort rawAverageCadence = BitConverter.ToUInt16(data, index);
                averageCadence = rawAverageCadence / 2.0;
                trainerState.AverageCadenceRpm = averageCadence;
                index += 2;
            }

            // -------------------------------------------------
            // Total Distance
            // -------------------------------------------------

            if ((flags & (1 << 4)) != 0)
            {
                uint rawDistance =
                    (uint)(data[index]
                    | (data[index + 1] << 8)
                    | (data[index + 2] << 16));

                distance = rawDistance / 1000.0;
                trainerState.DistanceKm = distance;
                index += 3;
            }

            // -------------------------------------------------
            // Resistance Level
            // -------------------------------------------------

            if ((flags & (1 << 5)) != 0)
            {
                byte rawResistance = data[index];
                resistanceLevel = rawResistance;
                trainerState.ResistanceLevel = resistanceLevel;
                index += 1;
            }

            // -------------------------------------------------
            // Instantaneous Power
            // -------------------------------------------------

            if ((flags & (1 << 6)) != 0)
            {
                short rawPower = BitConverter.ToInt16(data, index);
                power = rawPower;
                trainerState.PowerWatts = power;
                index += 2;
            }

            // -------------------------------------------------
            // Average Power
            // -------------------------------------------------

            if ((flags & (1 << 7)) != 0)
            {
                short rawAveragePower = BitConverter.ToInt16(data, index);
                averagePower = rawAveragePower;
                trainerState.AveragePowerWatts = averagePower;
                index += 2;
            }

            // -------------------------------------------------
            // Total Expended Energy
            // -------------------------------------------------
            
            if ((flags & (1 << 8)) != 0)
            {
                ushort rawTotalEnergy = BitConverter.ToUInt16(data, index);
                totalEnergy = rawTotalEnergy;
                trainerState.TotalEnergyKcal += totalEnergy;
                index += 2;

                // Skip Energy Per Hour
                index += 2;

                // Skip Energy Per Minute
                index += 1;
            }

            // -------------------------------------------------
            // Heart Rate
            // -------------------------------------------------

            if ((flags & (1 << 9)) != 0)
            {
                heartRate = data[index];
                trainerState.HeartRate = heartRate;
                index += 1;
            }
        }

        public TrainerCapabilities ParseFitnessMachineFeatures(byte[] data)
        {
            TrainerCapabilities caps = new TrainerCapabilities();

            // FTMS Fitness Machine Feature characteristic:
            //
            // Bytes 0-3 = Fitness Machine Features (uint32)
            // Bytes 4-7 = Target Setting Features (uint32)

            if (data == null || data.Length < 8)
                return null;

            uint machineFeatures = BitConverter.ToUInt32(data, 0);
            uint targetFeatures = BitConverter.ToUInt32(data, 4);

            /*
                FITNESS MACHINE FEATURES
                ------------------------

                Bit 0  = Average Speed Supported
                Bit 1  = Cadence Supported
                Bit 2  = Total Distance Supported
                Bit 3  = Inclination Supported
                Bit 5  = Heart Rate Measurement Supported
                Bit 14 = Power Measurement Supported
            */

            // -------------------------------------------------
            // Speed
            // -------------------------------------------------
            caps.SupportsSpeed =
                (machineFeatures & (1u << 0)) != 0;

            // -------------------------------------------------
            // Cadence
            // -------------------------------------------------
            caps.SupportsCadence =
                (machineFeatures & (1u << 1)) != 0;

            // -------------------------------------------------
            // Distance
            // -------------------------------------------------
            caps.SupportsDistance =
                (machineFeatures & (1u << 2)) != 0;

            // -------------------------------------------------
            // Inclination
            // -------------------------------------------------
            caps.SupportsInclination =
                (machineFeatures & (1u << 3)) != 0;

            // -------------------------------------------------
            // Heart Rate
            // -------------------------------------------------
            caps.SupportsHeartRate =
                (machineFeatures & (1u << 5)) != 0;

            // -------------------------------------------------
            // Power
            // -------------------------------------------------
            caps.SupportsPower =
                (machineFeatures & (1u << 14)) != 0;

            /*
                TARGET SETTING FEATURES
                -----------------------

                Bit 2 = Resistance Target Setting Supported
                Bit 3 = Power Target Setting Supported
                Bit 5 = Indoor Bike Simulation Parameters Supported
            */

            // -------------------------------------------------
            // Resistance Mode
            // -------------------------------------------------
            caps.SupportsResistance =
                (targetFeatures & (1u << 2)) != 0;

            // -------------------------------------------------
            // ERG Mode / Target Power
            // -------------------------------------------------
            caps.SupportsERG =
                (targetFeatures & (1u << 3)) != 0;

            // -------------------------------------------------
            // Simulation Mode
            // -------------------------------------------------
            caps.SupportsSimulation =
                (targetFeatures & (1u << 5)) != 0;

            return caps;
        }
    }
}

