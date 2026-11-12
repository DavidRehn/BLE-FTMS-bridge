using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.BLE.Enum
{
    public enum FitnessMachineControlOpcode : byte
    {
        RequestControl = 0x00,
        Reset = 0x01,
        SetTargetSpeed = 0x02,
        SetTargetInclination = 0x03,
        SetTargetResistanceLevel = 0x04,
        SetTargetPower = 0x05,
        SetTargetHeartRate = 0x06,
        StartOrResume = 0x07,
        StopOrPause = 0x08,
        SetTargetedExpendedEnergy = 0x09,
        SetTargetedNumberOfSteps = 0x0A,
        SetTargetedNumberOfStrides = 0x0B,
        SetTargetedDistance = 0x0C,
        SetTargetTrainingTime = 0x0D,
        SetTargetTimeInTwoHeartRateZones = 0x0E,
        SetTargetTimeInThreeHeartRateZones = 0x0F,
        SetTargetTimeInFiveHeartRateZones = 0x10,
        SetIndoorBikeSimulationParameters = 0x11,
        SetWheelCircumference = 0x12,
        SpinDownControl = 0x13,
        SetTargetedCadence = 0x14
    }
}
