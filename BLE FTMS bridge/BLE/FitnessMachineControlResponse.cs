using BLE_FTMS_bridge.BLE.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.BLE
{
    public class FitnessMachineControlResponse
    {
        public byte RequestOpcode { get; set; }
        public FitnessMachineControlResult ResultCode { get; set; }
        public byte[] ResponseParameters { get; set; } = Array.Empty<byte>();
        public bool IsSuccess => ResultCode == FitnessMachineControlResult.Success;
    }

}
