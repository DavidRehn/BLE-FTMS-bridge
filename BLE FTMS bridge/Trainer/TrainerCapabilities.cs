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

        // other
        public bool SupportsSimulation { get; set; }
        public bool SupportsWheelCircumference { get; set; }
        public bool SupportsSpinDown { get; set; }
        public bool SupportsTargetedCadence { get; set; }



        public TrainerCapabilities() { }
    }
}