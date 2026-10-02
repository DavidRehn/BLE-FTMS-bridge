using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (data == null || data.Length < 2)
                return;

            int index = 0;

            // First 2 bytes are flags
            ushort flags = BitConverter.ToUInt16(data, index);
            index += 2;


            //  ----------------------- Instantaneous Speed -----------------------

            bool speedPresent = (flags & 0x0001) == 0;

            if (speedPresent)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawSpeed = BitConverter.ToUInt16(data, index); 
                trainerState.SpeedKph = rawSpeed / 100.0;
                index += 2;
            }

            // ----------------------- Average Speed -----------------------

            if ((flags & (1 << 1)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawAverageSpeed = BitConverter.ToUInt16(data, index);
                trainerState.AverageSpeedKph = rawAverageSpeed / 100.0;
                index += 2;
            }

            // ----------------------- Instantaneous Cadence -----------------------

            if ((flags & (1 << 2)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawCadence = BitConverter.ToUInt16(data, index);
                trainerState.CadenceRpm = rawCadence / 2.0;
                index += 2;
            }

            // ----------------------- Average Cadence -----------------------

            if ((flags & (1 << 3)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawAverageCadence = BitConverter.ToUInt16(data, index);
                trainerState.AverageCadenceRpm = rawAverageCadence / 2.0;
                index += 2;
            }

            // ----------------------- Total Distance -----------------------

            if ((flags & (1 << 4)) != 0)
            {
                if (!HasBytes(data, index, 3))
                    return;
                uint rawDistance = (uint)(data[index] | (data[index + 1] << 8) | (data[index + 2] << 16));
                trainerState.DistanceKm = rawDistance / 1000.0;
                index += 3;
            }

            // ----------------------- Resistance Level -----------------------

            if ((flags & (1 << 5)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                short rawResistance = BitConverter.ToInt16(data, index);
                trainerState.ResistanceLevel = rawResistance;
                index += 2;
            }

            // ----------------------- Instantaneous Power -----------------------

            if ((flags & (1 << 6)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                short rawPower = BitConverter.ToInt16(data, index);
                trainerState.PowerWatts = rawPower;
                index += 2;
            }

            // ----------------------- Average Power -----------------------

            if ((flags & (1 << 7)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                short rawAveragePower = BitConverter.ToInt16(data, index);
                trainerState.AveragePowerWatts = rawAveragePower;
                index += 2;
            }

            // ----------------------- Total Expended Energy -----------------------

            if ((flags & (1 << 8)) != 0)
            {
                if (!HasBytes(data, index, 5))
                    return;
                ushort rawTotalEnergy = BitConverter.ToUInt16(data, index);
                trainerState.TotalEnergyKcal = rawTotalEnergy;

                index += 2; // Total Energy
                index += 2; // Energy Per Hour      (skip)
                index += 1; // Energy Per Minute    (skip)
            }

            // ----------------------- Heart Rate -----------------------

            if ((flags & (1 << 9)) != 0)
            {
                if (!HasBytes(data, index, 1))
                    return;
                trainerState.HeartRate = data[index];
                index += 1;
            }

            // ----------------------- Metabolic Equivalent -----------------------

            if ((flags & (1 << 10)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawMetabolicEquivalent = BitConverter.ToUInt16(data, index);
                trainerState.MetabolicEquivalent = rawMetabolicEquivalent / 10.0; 
                index += 2;
            }

            // ----------------------- Elapsed time -----------------------

            if ((flags & (1 << 11)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawElapsedTime = BitConverter.ToUInt16(data, index);
                trainerState.ElapsedTimeSeconds = rawElapsedTime;
                index += 2;
            }

            // ----------------------- Remaining time -----------------------

            if ((flags & (1 << 12)) != 0)
            {
                if (!HasBytes(data, index, 2))
                    return;
                ushort rawRemainingTime = BitConverter.ToUInt16(data, index);
                trainerState.RemainingTimeSeconds = rawRemainingTime;
                index += 2;
            }
        }

        public void ParseFitnessMachineFeatures(byte[] data, TrainerCapabilities trainerCapabilities)
        {
            if (data == null || data.Length < 8)
                return;

            uint machineFeatures = BitConverter.ToUInt32(data, 0);
            uint targetFeatures = BitConverter.ToUInt32(data, 4);

            /*
                FITNESS MACHINE FEATURES
                ------------------------ 
                Bit  0 = Average Speed
                Bit  1 = Cadence
                Bit  2 = Total Distance
                Bit  3 = Inclination
                Bit  7 = Resistance Level
                Bit  9 = Expended energy
                Bit 10 = Heart Rate Measurement
                Bit 11 = Metabolic Equivalent
                Bit 12 = Elapsed time
                Bit 13 = Remaining time
                Bit 14 = Power Measurement
            */


            // Average Speed 
            trainerCapabilities.SupportsAverageSpeed = (machineFeatures & (1u << 0)) != 0;

            // Cadence
            trainerCapabilities.SupportsCadence = (machineFeatures & (1u << 1)) != 0;

            // Distance 
            trainerCapabilities.SupportsDistance = (machineFeatures & (1u << 2)) != 0;

            // Inclination
            trainerCapabilities.SupportsInclination = (machineFeatures & (1u << 3)) != 0;

            // Resistance level
            trainerCapabilities.SupportsResistanceLevel = (machineFeatures & (1u << 7)) != 0;

            // Energy
            trainerCapabilities.SupportsEnergy = (machineFeatures & (1u << 9)) != 0;

            // Heartrate
            trainerCapabilities.SupportsHeartRate = (machineFeatures & (1u << 10)) != 0;

            // Metabolic Equivalent
            trainerCapabilities.SupportsMetabolicEquivalent = (machineFeatures & (1u << 11)) != 0;

            // Elapsed time
            trainerCapabilities.SupportsElapsedTime = (machineFeatures & (1u << 12)) != 0;

            // Remaining time 
            trainerCapabilities.SupportsRemainingTime = (machineFeatures & (1u << 13)) != 0;

            // Power
            trainerCapabilities.SupportsPower = (machineFeatures & (1u << 14)) != 0;

            /*
                TARGET SETTING FEATURES
                -----------------------
                Bit  0 = Speed Target Setting
                Bit  1 = Incline Target Setting
                Bit  2 = Resistance Target Setting 
                Bit  3 = Power Target Setting 
                Bit  4 = Heartrate Target Setting
                Bit 13 = Indoor Bike Simulation Parameters 
                Bit 14 = Wheel Circumference Configuration
                Bit 15 = Spin Down Control
                Bit 16 = Targeted Cadence
            */

            // Speed control
            trainerCapabilities.SupportsSpeedControl = (targetFeatures & (1u << 0)) != 0;

            // Inclination control
            trainerCapabilities.SupportsInclinationControl = (targetFeatures & (1u << 1)) != 0;

            // Resistance Mode 
            trainerCapabilities.SupportsResistanceControl = (targetFeatures & (1u << 2)) != 0;

            // Power control
            trainerCapabilities.SupportsPowerControl =  (targetFeatures & (1u << 3)) != 0;

            // Heartrate control
            trainerCapabilities.SupportsHeartRateControl = (targetFeatures & (1u << 4)) != 0;

            // Simulation Mode
            trainerCapabilities.SupportsSimulation = (targetFeatures & (1u << 13)) != 0;

            // Wheel Circumference
            trainerCapabilities.SupportsWheelCircumference = (targetFeatures & (1u << 14)) != 0;

            // Spin down
            trainerCapabilities.SupportsSpinDown = (targetFeatures & (1u << 15)) != 0;

            // Targeted Cadence
            trainerCapabilities.SupportsTargetedCadence = (targetFeatures & (1u << 16)) != 0;
        }

        private static bool HasBytes(byte[] data, int index, int count)
        {
            return index + count <= data.Length;
        }
    }
}

