using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeClient.SignalR.Helper {
    public static class ConnectionManager {
        public static HubConnection BuildConnection(
                string connectionURL, 
                string userName)
        {
            // Ensure WebSocket and SkipNegotiation to work under Server Fram
            var connection = new HubConnectionBuilder()
            .WithUrl($"{connectionURL}?username={userName}", options => {
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;
            })
            .AddMessagePackProtocol()
            .WithAutomaticReconnect()
            .Build();

            connection.Closed += async (error) =>
            {
                Console.WriteLine("Connection Closed");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                Console.WriteLine("Attempting to Reconnect");
                await connection.StartAsync();
                Console.WriteLine("Connection started");
            };

            connection.Reconnecting += error =>
            {
                Console.WriteLine("Reconnecting");
                return Task.CompletedTask;
            };

            connection.Reconnected += _ =>
            {
                Console.WriteLine("Reconnected");
                return Task.CompletedTask;
            };

            return connection;
        }

        public static async void StartConnection<S, T>(
            HubConnection connection, 
            string receiveName, 
            string prependedMessage = "", 
            bool start = true)
        {
            connection.On<S, T>(receiveName, (S user, T message) =>
            {
                var newMessage = string.Join(
                    Environment.NewLine,
                    $"{prependedMessage}", 
                    $"  UserName: {user?.ToString()}", 
                    $"{message?.ToString()}"
                );
                Console.WriteLine("***************************");
                Console.WriteLine(newMessage.ToString());
                Console.WriteLine("***************************");
            });

            if (start)
            {
                try
                {
                    await connection.StartAsync();
                    Console.WriteLine("Connection started");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}