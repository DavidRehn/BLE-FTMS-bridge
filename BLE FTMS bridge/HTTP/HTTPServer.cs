using BLE_FTMS_bridge.Trainer;
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
        private TrainerStatus trainer;
        private HttpListener httpListener;
        private const string url = "http://localhost:8000/";
        public bool isRunning;

        public HTTPServer(TrainerStatus trainer)
        {
            this.trainer = trainer;
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(url);
            isRunning = true;
        }

        public async Task Start()
        {
            httpListener.Start();
            while (isRunning)
            {
                HttpListenerContext ctx = await httpListener.GetContextAsync(); // Contains request and response
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Shut down the server
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    isRunning = false;
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
            }
        }

        private async Task RespondJSON(string json, HttpListenerResponse response)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            response.StatusCode = 200;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.Length;
            await response.OutputStream.WriteAsync(data);
            response.Close();
        }
    }
}
