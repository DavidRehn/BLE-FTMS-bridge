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
        // Supported data
        public bool SupportsSpeed { get; set; }
        public bool SupportsCadence { get; set; }
        public bool SupportsPower { get; set; }
        public bool SupportsHeartRate { get; set; }
        public bool SupportsDistance { get; set; }

        // Control support
        public bool SupportsResistance { get; set; }    
        public bool SupportsERG { get; set; }   // Target power
        public bool SupportsSimulation { get; set; }    //Gradient, wind,  rolling resistance, drag

        // Other
        public bool SupportsInclination { get; set; }   // Reported gradient

        public TrainerCapabilities() { }
    }
}