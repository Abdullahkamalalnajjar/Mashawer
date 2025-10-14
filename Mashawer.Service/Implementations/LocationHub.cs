using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Mashawer.Service.Implementations
{
    public class LocationHub : Hub
    {
        // نخزن المستخدمين المتصلين (userId -> connectionId)
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId!] = Context.ConnectionId;
                Console.WriteLine($"✅ User connected: {userId} ({Context.ConnectionId})");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(user.Key))
            {
                _connections.TryRemove(user.Key, out _);
                Console.WriteLine($"❌ User disconnected: {user.Key}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // السائق يرسل موقعه
        public async Task UpdateLocation(string driverId, string userId, double latitude, double longitude)
        {
            if (_connections.TryGetValue(userId, out var userConnectionId))
            {
                await Clients.Client(userConnectionId)
                    .SendAsync("ReceiveLocation", driverId, latitude, longitude);
            }
            else
            {
                Console.WriteLine($"⚠️ User {userId} not connected!");
            }
        }
    }
}