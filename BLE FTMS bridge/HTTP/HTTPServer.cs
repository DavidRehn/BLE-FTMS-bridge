using BLE_FTMS_bridge.Trainer;
using BLETestApp.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace BLE_FTMS_bridge.HTTP
{
    public class HTTPServer
    {
        private BLEScanner scanner;
        private BLEConnection conn;

        private ProgramState programState;
        private TrainerStatus trainer;  

        private HttpListener httpListener;
        private const string url = "http://localhost:8000/";

        public HTTPServer(TrainerStatus trainer, ProgramState programState)
        {
            this.programState = programState;
            this.trainer = trainer;

            scanner = new BLEScanner();
            conn = new BLEConnection(scanner, this.trainer);

            httpListener = new HttpListener();
            httpListener.Prefixes.Add(url);
        }

        public async Task Start()
        {
            Init();
            while (programState.IsRunning)
            {
                HttpListenerContext ctx = await httpListener.GetContextAsync(); // Contains request and response
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Shut down the server
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    programState.IsRunning = false;
                    await RespondJSON("{\"success\":true}", resp);
                    return;
                }
                // Request for trainer status
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/trainer_status"))
                {
                    string json = JsonSerializer.Serialize(trainer.trainerState);
                    await RespondJSON(json, resp);
                }
                // Request for trainer capabilities
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/trainer_capabilities"))
                {
                    string json = JsonSerializer.Serialize(trainer.trainerCapabilities);
                    await RespondJSON(json, resp);
                }
                // request for connectable devices
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/devices"))
                {
                    string json = JsonSerializer.Serialize(scanner.GetDevices());
                    await RespondJSON(json, resp);
                }
                else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/connect"))
                {
                    using StreamReader reader = new StreamReader(req.InputStream);
                    string body = await reader.ReadToEndAsync();                        // received body

                    try
                    {
                        ConnectRequest? request = JsonSerializer.Deserialize<ConnectRequest>(body);

                        if (request == null)
                        {
                            await RespondJSON("{\"success\":false,\"error\":\"Invalid request\"}", resp, 400);
                            continue;
                        }

                        bool success = await conn.ConnectDevice(request.Address);
                        int status = success ? 200 : 400;
                        body = success ? "{\"success\":true}" : "{\"success\":false}";    // body of response
                        await RespondJSON(body, resp, status);
                    }
                    catch (JsonException e)
                    {
                        await RespondJSON("{\"success\":false,\"error\":\"Invalid address\"}", resp, 400);
                    }
                }
                else
                {
                    await RespondJSON("{\"success\":false,\"error\":\"Invalid command\"}", resp, 404);  // For commands that doesn't match any of the above
                }
            }
            httpListener.Stop();

        }

        private async Task RespondJSON(string json, HttpListenerResponse response, int statusCode = 200)    // statusCode defaults to 200
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.Length;
            await response.OutputStream.WriteAsync(data);
            response.Close();
        }

        private void Init()
        {
            httpListener.Start();
            scanner.StartScanning();
        }
    }
}
