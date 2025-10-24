using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class MainHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
            
        }

        public async Task SendPayloadAll(string payload)
        {
            var data = JsonSerializer.Deserialize<dynamic>(payload);
            string json = JsonSerializer.Serialize(data);
            await Clients.All.SendAsync("ReceivePayloadAll", json);
        }

        public async Task SendPayloadCaller(string payload)
        {
            var data = JsonSerializer.Deserialize<dynamic>(payload);
            string json = JsonSerializer.Serialize(data);
            await Clients.Caller.SendAsync("ReceivePayloadCaller", json);
        }

        // Binary data methods
        public async Task SendBinaryAll(byte[] binaryData)
        {
            try
            {
                Console.WriteLine($"Try decoding binary data of {binaryData.Length} bytes from {Context.ConnectionId}: {Encoding.UTF8.GetString(binaryData)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error decoding binary data of {binaryData.Length} bytes from {Context.ConnectionId}: {e.Message}");
            }
            await Clients.All.SendAsync("ReceiveBinaryAll", binaryData);
        }

        public async Task SendBinaryCaller(byte[] binaryData)
        {
            try
            {
                Console.WriteLine($"Try decoding binary data of {binaryData.Length} bytes from {Context.ConnectionId}: {Encoding.UTF8.GetString(binaryData)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error decoding binary data of {binaryData.Length} bytes from {Context.ConnectionId}: {e.Message}");
            }
            await Clients.Caller.SendAsync("ReceiveBinaryCaller", binaryData);
        }
    }
}
