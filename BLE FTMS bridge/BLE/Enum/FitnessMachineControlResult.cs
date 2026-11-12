using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.BLE.Enum
{
    public enum FitnessMachineControlResult : byte
    {
        Success = 0x01,
        OpCodeNotSupported = 0x02,
        InvalidParameter = 0x03,
        OperationFailed = 0x04,
        ControlNotPermitted = 0x05
    }
}
