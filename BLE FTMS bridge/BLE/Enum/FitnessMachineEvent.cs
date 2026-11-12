using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.BLE.Enum
{
    public enum FitnessMachineEvent : byte
    {
        Reset = 0x01,
        StoppedOrPausedByUser = 0x02,
        StoppedBySafetyKey = 0x03,
        StartedOrResumedByUser = 0x04,
        TargetSpeedChanged = 0x05,
        TargetInclinationChanged = 0x06,
        TargetResistanceChanged = 0x07,
        TargetPowerChanged = 0x08,
        TargetHeartRateChanged = 0x09,
        TargetEnergyChanged = 0x0A,
        TargetStepsChanged = 0x0B,
        TargetStridesChanged = 0x0C,
        TargetDistanceChanged = 0x0D,
        TargetTrainingTimeChanged = 0x0E,
        TargetTimeInTwoHeartRateZonesChanged = 0x0F,
        TargetTimeInThreeHeartRateZonesChanged = 0x10,
        TargetTimeInFiveHeartRateZonesChanged = 0x11,
        SimulationParametersChanged = 0x12,
        WheelCircumferenceChanged = 0x13,
        SpinDownStatus = 0x14,
        TargetCadenceChanged = 0x15
    }
}
