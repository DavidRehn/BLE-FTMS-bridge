using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.BLE.Enum
{
    public enum SpinDownStatus
    {
        NotStarted,
        Requested,
        InProgress,
        StopPedaling,
        Success,
        Error
    }
}
