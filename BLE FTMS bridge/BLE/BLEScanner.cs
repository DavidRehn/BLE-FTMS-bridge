using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace BLETestApp.BLE
{
    public class BLEScanner
    {
        private readonly int advTTL = 10; // Time a device remains in list after last advertisement
        private BluetoothLEAdvertisementWatcher watcher;
        private List<BLEDevice> connectableDevices;  // Saved advertisements from compatible devices

        public BLEScanner()
        {
            watcher = new BluetoothLEAdvertisementWatcher();
            connectableDevices = new List<BLEDevice>();
        }

        public void StartScanning()
        {
            watcher.Received += (BTAdvWatcher, btAdv) =>
            { // Executed when advertisement is received
              // Saves all compatible devices
                if (HasFTMS(btAdv.Advertisement) /*|| HasCPS(btAdv.Advertisement)*/)
                {
                    BLEDevice device = new BLEDevice(btAdv.Advertisement.LocalName, btAdv.BluetoothAddress);
                    if (connectableDevices.Contains(device)) //Does not take TTL into account
                    {
                        connectableDevices.Remove(device);// For updating TTL
                    }
                    connectableDevices.Add(device);
                }
            };
            watcher.Start();
        }

        public void StopScanning()
        {
            watcher.Stop();
        }

        // Checks if the scanned device has CPS or FTMS services
        private static bool HasFTMS(BluetoothLEAdvertisement btAdv)
        {
            var uuids = btAdv.ServiceUuids;
            return uuids.Contains(Guid.Parse("00001826-0000-1000-8000-00805f9b34fb"));  // UUID for FTMS
        }

        // Checks if the scanned device has CPS or FTMS services
        /*private static bool HasCPS(BluetoothLEAdvertisement btAdv)
        {
            var uuids = btAdv.ServiceUuids;
            return uuids.Contains(Guid.Parse("00001818-0000-1000-8000-00805f9b34fb"));  // UUID for CPS
        }*/

        // For use in a seperate thread, removes expires devices from saved set
        public void RemoveOldAdvertisements()
        {
            try
            {
                foreach (BLEDevice device in connectableDevices)
                {
                    if (device.lastAdvertisementTime.AddSeconds(advTTL).CompareTo(DateTime.Now) < 0) // If time since added exceeds TTL
                    {
                        connectableDevices.Remove(device);
                    }
                }
            }catch (Exception e)
            {

            }
        }

        public BLEDevice GetDevice(int i)
        {
            return connectableDevices[i];
        }
    }
}
