using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.Trainer
{
    public class TrainerStatus
    {
        public TrainerStatus()
        {
            trainerState = new TrainerState();
            trainerCapabilities = new TrainerCapabilities();
        }

        public TrainerCapabilities trainerCapabilities { get; set; }
        public TrainerState trainerState { get; set; }
    }
}
