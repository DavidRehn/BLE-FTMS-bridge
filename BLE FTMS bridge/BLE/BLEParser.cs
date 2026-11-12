using BLE_FTMS_bridge.Trainer;
using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BLE_FTMS_bridge.BLE.Enum;

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

        public void ParseFitnessMachineStatus(byte[] data, TrainerStatus trainerStatus, TrainerConfiguration trainerConfiguration)
        {
            if (data == null || data.Length < 1 || trainerStatus == null || trainerConfiguration == null)
                return;

            int index = 1;
            byte opcode = data[0];
            FitnessMachineEvent? parsedEvent = null;

            switch (opcode)
            {
                // ----------------------- Reset -----------------------

                case 0x01:
                    parsedEvent = FitnessMachineEvent.Reset;
                    trainerStatus.IsRunning = false;
                    trainerStatus.IsPaused = false;
                    break;

                // ----------------------- Stopped / Paused by User -----------------------

                case 0x02:
                    {
                        if (!HasBytes(data, index, 1))
                            return;

                        byte controlInformation = data[index++];

                        if (controlInformation == 0x01) // Stop
                        {
                            parsedEvent = FitnessMachineEvent.StoppedOrPausedByUser;

                            trainerStatus.IsRunning = false;
                            trainerStatus.IsPaused = false;
                        }
                        else if (controlInformation == 0x02) // Pause
                        {
                            parsedEvent = FitnessMachineEvent.StoppedOrPausedByUser;

                            trainerStatus.IsRunning = false;
                            trainerStatus.IsPaused = true;
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }

                // ----------------------- Stopped by Safety Key -----------------------

                case 0x03:
                    parsedEvent = FitnessMachineEvent.StoppedBySafetyKey;
                    trainerStatus.IsRunning = false;
                    trainerStatus.IsPaused = false;
                    break;

                // ----------------------- Started / Resumed by User -----------------------

                case 0x04:
                    parsedEvent = FitnessMachineEvent.StartedOrResumedByUser;
                    trainerStatus.IsRunning = true;
                    trainerStatus.IsPaused = false;
                    break;

                // ----------------------- Target Speed Changed -----------------------

                case 0x05:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        ushort rawSpeed = BitConverter.ToUInt16(data, index);

                        trainerConfiguration.TargetSpeedKph = rawSpeed / 100.0;

                        parsedEvent = FitnessMachineEvent.TargetSpeedChanged;
                        break;
                    }

                // ----------------------- Target Inclination Changed -----------------------

                case 0x06:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        short rawInclination = BitConverter.ToInt16(data, index);

                        trainerConfiguration.TargetInclination = rawInclination / 10.0;

                        parsedEvent = FitnessMachineEvent.TargetInclinationChanged;
                        break;
                    }

                // ----------------------- Target Resistance Changed -----------------------

                case 0x07:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        short rawResistance = BitConverter.ToInt16(data, index);

                        trainerConfiguration.TargetResistance = rawResistance / 10.0;

                        parsedEvent = FitnessMachineEvent.TargetResistanceChanged;
                        break;
                    }

                // ----------------------- Target Power Changed -----------------------

                case 0x08:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        short rawPower = BitConverter.ToInt16(data, index);

                        trainerConfiguration.TargetPowerWatts = rawPower;

                        parsedEvent = FitnessMachineEvent.TargetPowerChanged;
                        break;
                    }

                // ----------------------- Target Heart Rate Changed -----------------------

                case 0x09:
                    {
                        if (!HasBytes(data, index, 1))
                            return;

                        trainerConfiguration.TargetHeartRate = data[index];

                        parsedEvent = FitnessMachineEvent.TargetHeartRateChanged;
                        break;
                    }

                // ----------------------- Target Expended Energy Changed -----------------------

                case 0x0A:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        parsedEvent = FitnessMachineEvent.TargetEnergyChanged;
                        break;
                    }

                // ----------------------- Targeted Number of Steps Changed -----------------------

                case 0x0B:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        parsedEvent = FitnessMachineEvent.TargetStepsChanged;
                        break;
                    }

                // ----------------------- Targeted Number of Strides Changed -----------------------

                case 0x0C:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        parsedEvent = FitnessMachineEvent.TargetStridesChanged;
                        break;
                    }

                // ----------------------- Target Distance Changed -----------------------

                case 0x0D:
                    {
                        if (!HasBytes(data, index, 3))
                            return;

                        parsedEvent = FitnessMachineEvent.TargetDistanceChanged;
                        break;
                    }

                // ----------------------- Target Training Time Changed -----------------------

                case 0x0E:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        parsedEvent = FitnessMachineEvent.TargetTrainingTimeChanged;
                        break;
                    }

                // ----------------------- Target Time in Heart Rate Zones -----------------------

                case 0x0F:
                case 0x10:
                case 0x11:
                    {
                        int parameterLength = opcode == 0x0F ? 4 : opcode == 0x10 ? 6 : 10;

                        if (!HasBytes(data, index, parameterLength))
                            return;

                        if (opcode == 0x0F)
                        {
                            parsedEvent = FitnessMachineEvent.TargetTimeInTwoHeartRateZonesChanged;
                        }
                        else if (opcode == 0x10)
                        {
                            parsedEvent = FitnessMachineEvent.TargetTimeInThreeHeartRateZonesChanged;
                        }
                        else
                        {
                            parsedEvent = FitnessMachineEvent.TargetTimeInFiveHeartRateZonesChanged;
                        }
                        break;
                    }

                // ----------------------- Simulation Parameters Changed -----------------------

                case 0x12:
                    {
                        if (!HasBytes(data, index, 6))
                            return;

                        short rawWindSpeed = BitConverter.ToInt16(data, index);
                        short rawGrade = BitConverter.ToInt16(data, index + 2);
                        byte rawRollingResistance = data[index + 4];
                        byte rawWindResistance = data[index + 5];

                        trainerConfiguration.WindSpeed = rawWindSpeed / 1000.0;
                        trainerConfiguration.Grade = rawGrade / 100.0;
                        trainerConfiguration.RollingResistanceCoefficient = rawRollingResistance / 10000.0;
                        trainerConfiguration.WindResistanceCoefficient = rawWindResistance / 100.0;

                        parsedEvent = FitnessMachineEvent.SimulationParametersChanged;
                        break;
                    }

                // ----------------------- Wheel Circumference Changed -----------------------

                case 0x13:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        ushort rawCircumference = BitConverter.ToUInt16(data, index);

                        trainerConfiguration.WheelCircumferenceMm = rawCircumference / 10.0;

                        parsedEvent = FitnessMachineEvent.WheelCircumferenceChanged;
                        break;
                    }

                // ----------------------- Spin Down Status -----------------------

                case 0x14:
                    {
                        if (!HasBytes(data, index, 1))
                            return;

                        trainerConfiguration.SpinDownStatusValue = data[index];

                        parsedEvent = FitnessMachineEvent.SpinDownStatus;
                        break;
                    }

                // ----------------------- Target Cadence Changed -----------------------

                case 0x15:
                    {
                        if (!HasBytes(data, index, 2))
                            return;

                        ushort rawCadence = BitConverter.ToUInt16(data, index);
                        trainerConfiguration.TargetCadenceRpm = rawCadence / 2.0;

                        parsedEvent = FitnessMachineEvent.TargetCadenceChanged;
                        break;
                    }
                default:
                    return;
            }
            trainerStatus.LastEvent = parsedEvent;
            return;
        }

        private static bool HasBytes(byte[] data, int index, int count)
        {
            return index + count <= data.Length;
        }
    }
}

