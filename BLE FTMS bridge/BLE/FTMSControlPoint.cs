using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BLE_FTMS_bridge.BLE.Enum;

namespace BLE_FTMS_bridge.BLE
{
    public class FTMSControlPoint
    {
        private readonly GattCharacteristic controlPoint;
        private readonly SemaphoreSlim commandLock =  new SemaphoreSlim(1, 1);
        private TaskCompletionSource<FitnessMachineControlResponse>? pendingResponse;
        private byte? pendingOpcode;
        public bool HasControlPermission { get; private set; }


        public FTMSControlPoint(GattCharacteristic characteristic)
        {
            controlPoint = characteristic ?? throw new ArgumentNullException(nameof(characteristic));

            controlPoint.ValueChanged += ControlPointValueChanged;
        }

        private void ControlPointValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            using (DataReader reader =  DataReader.FromBuffer(args.CharacteristicValue))
            {
                byte[] data = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(data);

                // Minimum response length is 3 bytes.
                if (data.Length < 3)
                    return;

                // Response Code must be 0x80.
                if (data[0] != 0x80)
                    return;

                byte requestOpcode = data[1];

                var response = new FitnessMachineControlResponse
                {
                    RequestOpcode = requestOpcode,
                    ResultCode = (FitnessMachineControlResult)data[2],
                    ResponseParameters = data.Skip(3).ToArray()
                };

                // Ignore responses that don't match the pending request.
                if (pendingResponse == null || pendingOpcode != requestOpcode)
                    return;

                // Update control permission.
                if (requestOpcode == 0x00 && response.IsSuccess)
                    HasControlPermission = true;

                if (requestOpcode == 0x01 && response.IsSuccess)
                    HasControlPermission = false;

                pendingResponse.TrySetResult(response);

                Console.WriteLine("Control Point indication: " + BitConverter.ToString(data));
            }
        }


        public async Task<FitnessMachineControlResponse> ExecuteCommandAsync(byte[] command, int timeoutMs = 10000)
        {
            if (command == null || command.Length == 0)
                throw new ArgumentException("Invalid command.");

            byte opcode = command[0];

            // Only Request Control can be sent without control permission.
            if (opcode != 0x00 && !HasControlPermission)
                throw new InvalidOperationException("Trainer control permission has not been granted.");

            await commandLock.WaitAsync();

            try
            {
                var tcs = new TaskCompletionSource<FitnessMachineControlResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

                pendingOpcode = opcode;
                pendingResponse = tcs;

                var writer = new DataWriter();
                writer.WriteBytes(command);

                var writeStatus = await controlPoint.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithResponse);

                if (writeStatus != GattCommunicationStatus.Success)
                    throw new InvalidOperationException($"Control Point write failed: {writeStatus}");

                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

                if (completedTask != tcs.Task)
                    throw new TimeoutException("Trainer did not respond to Control Point command.");

                return await tcs.Task;
            }
            finally
            {
                pendingResponse = null;
                pendingOpcode = null;

                commandLock.Release();
            }
        }

        public Task<FitnessMachineControlResponse> RequestControlAsync()
        {
            return ExecuteCommandAsync(new byte[] { 0x00 });
        }

        public Task<FitnessMachineControlResponse> ResetAsync()
        {
            return ExecuteCommandAsync(new byte[] { 0x01 });
        }

        public Task<FitnessMachineControlResponse> StartOrResumeAsync()
        {
            return ExecuteCommandAsync(new byte[] { 0x07 });
        }

        public Task<FitnessMachineControlResponse> StopAsync()
        {
            return ExecuteCommandAsync(new byte[] { 0x08, 0x01 });
        }

        public Task<FitnessMachineControlResponse> PauseAsync()
        {
            return ExecuteCommandAsync(new byte[] { 0x08, 0x02 });
        }

        public Task<FitnessMachineControlResponse> SetTargetPowerAsync(short watts)
        {
            byte[] parameters = BitConverter.GetBytes(watts);

            return ExecuteCommandAsync(new byte[]
            {
                0x05,
                parameters[0],
                parameters[1]
            });
        }

        public Task<FitnessMachineControlResponse> SetTargetResistanceAsync(double resistance)
        {
            short rawValue = checked((short)Math.Round(resistance * 10.0));

            byte[] parameters = BitConverter.GetBytes(rawValue);

            return ExecuteCommandAsync(new byte[]
            {
                0x04,
                parameters[0],
                parameters[1]
            });
        }

        // Call when trainer disconnects
        public void OnDisconnected()
        {
            HasControlPermission = false;

            pendingResponse?.TrySetException(
                new InvalidOperationException("Trainer disconnected."));
        }

        // Call when connection is permanently disposed
        public void Dispose()
        {
            controlPoint.ValueChanged -= ControlPointValueChanged;

            OnDisconnected();
        }
    }
}
