
using BLE_FTMS_bridge;
using BLE_FTMS_bridge.HTTP;
using BLE_FTMS_bridge.Trainer;
using BLETestApp.BLE;
using System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Notifications;

class Program
{

    static async Task Main(string[] args)
    {
        ProgramState programState = new ProgramState();
        TrainerStatus trainer = new TrainerStatus();

        HTTPServer httpServer = new HTTPServer(trainer, programState);
        await httpServer.Start();
    }
}