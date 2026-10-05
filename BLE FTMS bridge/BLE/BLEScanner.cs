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
        private readonly int advTTL = 10; // Time a device remains in list after last advertisement (seconds)
        private readonly BluetoothLEAdvertisementWatcher watcher;
        private readonly object devicesLock = new object();
        private readonly List<BLEDevice> connectableDevices;  // Saved advertisements from compatible devices
        private Thread cleanupThread;

        public BLEScanner()
        {
            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.Received += WatcherReceived;
            connectableDevices = new List<BLEDevice>();

            cleanupThread = new Thread(() =>
            {
                while (true)
                {
                    RemoveOldAdvertisements();
                    Thread.Sleep(1000);
                }
            });

            cleanupThread.IsBackground = true;
            cleanupThread.Start();

            StartScanning();
        }

        public void StartScanning()
        {
            if (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                return;
            watcher.Start();
        }


        public void StopScanning()
        {
            if (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                watcher.Stop();
        }


        // Checks if the scanned device has FTMS service.
        private static bool HasFTMS(BluetoothLEAdvertisement btAdv)
        {
            var uuids = btAdv.ServiceUuids;
            return uuids.Contains(Guid.Parse("00001826-0000-1000-8000-00805f9b34fb"));  // UUID for FTMS
        }



        // For use in a seperate thread, removes expires devices from saved set
        public void RemoveOldAdvertisements()
        {
            lock (devicesLock)
            {
                connectableDevices.RemoveAll(device =>
                    device.lastAdvertisementTime.AddSeconds(advTTL) < DateTime.Now);
            }
        }


        public List<BLEDevice> GetDevices()
        {
            lock (devicesLock)
            {
                return new List<BLEDevice>(connectableDevices);
            }
        }


        private void WatcherReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            // Executed when advertisement is received
            // Saves all compatible devices
            if (HasFTMS(btAdv.Advertisement))
            {
                BLEDevice device = new BLEDevice(btAdv.Advertisement.LocalName, btAdv.BluetoothAddress);
                lock (devicesLock)
                {
                    if (connectableDevices.Contains(device)) //Does not take TTL into account
                    {
                        connectableDevices.Remove(device);// For updating TTL
                    }
                    connectableDevices.Add(device);
                }
            }
        }
    }
}
