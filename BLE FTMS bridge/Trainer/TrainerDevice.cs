using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.Trainer
{
    public class TrainerDevice
    {
        public TrainerDevice()
        {
            trainerState = new TrainerState();                  // Indoor bike data
            trainerCapabilities = new TrainerCapabilities();    // Fitness machine features / Target setting features   
        }

        public TrainerCapabilities trainerCapabilities { get; set; }
        public TrainerState trainerState { get; set; }
        public TrainerStatus trainerStatus { get; set; }
    }
}
