using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.Trainer
{
    public class TrainerConfiguration
    {
        public TargetMode ControlMode { get; set; }

        // ----------------------- Target Values -----------------------

        public double? TargetSpeedKph { get; set; }
        public double? TargetInclination { get; set; }
        public double? TargetResistance { get; set; }
        public short? TargetPowerWatts { get; set; }
        public byte? TargetHeartRate { get; set; }
        public double? TargetCadenceRpm { get; set; }



        // ----------------------- Simulation Parameters -----------------------

        public double? WindSpeed { get; set; }
        public double? Grade { get; set; }
        public double? RollingResistanceCoefficient { get; set; }
        public double? WindResistanceCoefficient { get; set; }


        // ----------------------- Other -----------------------

        public double? WheelCircumferenceMm { get; set; }
        public byte? SpinDownStatusValue { get; set; }
        public short? SpinDownResult { get; set; }


        public TrainerConfiguration()
        {
            ControlMode = TargetMode.None;
            TargetSpeedKph = null;
            TargetInclination = null;
            TargetResistance = null;
            TargetPowerWatts = null;
            TargetHeartRate = null;
            TargetCadenceRpm = null;
            WindSpeed = null;
            Grade = null;
            RollingResistanceCoefficient = null;
            WindResistanceCoefficient = null;
            WheelCircumferenceMm = null;
            SpinDownStatusValue = null;
            SpinDownResult = null;
        }

        public void Reset()
        {
            TargetSpeedKph = null;
            TargetInclination = null;
            TargetResistance = null;
            TargetPowerWatts = null;
            TargetHeartRate = null;
            TargetCadenceRpm = null;
            WindSpeed = null;
            Grade = null;
            RollingResistanceCoefficient = null;
            WindResistanceCoefficient = null;
        }
    }
}
