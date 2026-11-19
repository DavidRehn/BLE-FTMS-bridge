using BLE_FTMS_bridge.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace BLETestApp.Trainer
{
    public class TrainerCapabilities
    {
        // Measurements
        public bool SupportsAverageSpeed { get; set; }
        public bool SupportsCadence { get; set; }
        public bool SupportsDistance { get; set; }
        public bool SupportsInclination { get; set; }
        public bool SupportsResistanceLevel { get; set; }
        public bool SupportsEnergy { get; set; }
        public bool SupportsHeartRate { get; set; }
        public bool SupportsMetabolicEquivalent { get; set; }
        public bool SupportsElapsedTime { get; set; }
        public bool SupportsRemainingTime { get; set; }
        public bool SupportsPower { get; set; }


        // Control
        
        // targets
        public bool SupportsSpeedControl { get; set; }
        public bool SupportsInclinationControl { get; set; }
        public bool SupportsResistanceControl { get; set; }
        public bool SupportsPowerControl { get; set; }
        public bool SupportsHeartRateControl { get; set; }
        public bool SupportsTargetedCadence { get; set; }

        // other
        public bool SupportsSimulation { get; set; }
        public bool SupportsWheelCircumference { get; set; }
        public bool SupportsSpinDown { get; set; }


        // Supported ranges

        // Resistance (resolution: 0.1)
        public double MinResistance { get; set; }
        public double MaxResistance { get; set; }
        public double ResistanceIncrement { get; set; }

        // Heart rate (resolution: 1 bpm)
        public byte MinHeartRate { get; set; }
        public byte MaxHeartRate { get; set; }
        public byte HeartRateIncrement { get; set; }

        // Speed (resolution: 0.01 km/h)
        public double MinSpeedKph { get; set; }
        public double MaxSpeedKph { get; set; }
        public double SpeedIncrementKph { get; set; }

        // Inclination (resolution: 0.1%)
        public double MinInclination { get; set; }
        public double MaxInclination { get; set; }
        public double InclinationIncrement { get; set; }

        // Power (resolution: 1 W)
        public short MinPowerWatts { get; set; }
        public short MaxPowerWatts { get; set; }
        public short PowerIncrementWatts { get; set; }


    }
}