using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_FTMS_bridge.HTTP.API_Requests
{
    public class SetSimulationRequest
    {
        public double WindSpeed { get; set; }
        public double Grade { get; set; }
        public double RollingResistanceCoefficient { get; set; }
        public double WindResistanceCoefficient { get; set; }
    }
}
