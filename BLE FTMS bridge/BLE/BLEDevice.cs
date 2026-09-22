using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLETestApp.BLE
{
    public class BLEDevice
    {
        public string localName { get; }
        public ulong address { get; }
        public DateTime lastAdvertisementTime { get; }  // Last time an advertisement from this device was received
        public BLEDevice(string localName, ulong address)
        {
            this.localName = localName;
            this.address = address;
            lastAdvertisementTime = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is BLEDevice)) return false;
            BLEDevice device = (BLEDevice)obj;
            return device.localName == localName && device.address == address;
        }

        public override string ToString()
        {
            return "Localname: " + localName + "  Address: " + address;
        }
    }
}
