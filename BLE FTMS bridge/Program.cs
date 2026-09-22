
using BLETestApp.BLE;
using System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Notifications;

class Program
{

    static void Main(string[] args)
    {
        BLEScanner scanner = new BLEScanner();
        BLEConnection conn = new BLEConnection(scanner);
        scanner.StartScanning();
        Console.WriteLine("Scanning... press Enter to stop");
        String input = Console.ReadLine();
        Temp(scanner, conn, input);
        Console.ReadLine();
    }

    private static async Task Temp(BLEScanner scanner, BLEConnection conn, string input)
    {
        try
        {
            // Chose device to connect to (temporary)
            int i = Int32.Parse(input);
            await conn.ConnectDevice(scanner.GetDevice(i).address);
        }
        catch
        {
            Console.WriteLine("Connection failed");
        }

        scanner.StopScanning();
        await conn.ConnectionSetup();
    }
}