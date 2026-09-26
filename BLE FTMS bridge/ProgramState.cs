using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge
{
    public class ProgramState
    {
        public bool IsRunning { get; set; }

        public ProgramState()
        {
            IsRunning = true;
        }
    }
}
