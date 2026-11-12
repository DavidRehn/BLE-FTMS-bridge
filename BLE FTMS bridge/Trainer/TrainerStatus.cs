using BLETestApp.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BLE_FTMS_bridge.BLE.Enum;

namespace BLE_FTMS_bridge.Trainer
{
    public class TrainerStatus
    {
        // Last received event
        public FitnessMachineEvent? LastEvent { get; set; }

        // Trainer operating state
        public bool? IsRunning { get; set; }
        public bool? IsPaused { get; set; }
    }
}
