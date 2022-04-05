using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using RealtimeClient.SignalR.Helper;

namespace RealtimeClient.SignalR
{
    public class VectorSyncClient {
        private static ManualResetEvent _quitEvent = new (false);
        private readonly string _userName;
        private readonly string _serverURL;
        private readonly bool _vectorLoop;
        private static bool keepRunning = true;

        public VectorSyncClient(
            string serverURL,
            string userName,
            bool vectorLoop)
        {
            _userName = userName;
            _serverURL = serverURL;
            _vectorLoop = vectorLoop;

            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                keepRunning = false;                
                eArgs.Cancel = true;
            };
        }

        public void Run(IEnumerable<string> spaces, bool listenOnly)
        {
            var chatConnection = ConnectionManager.BuildConnection(
                $"{_serverURL}/ChatHub", _userName);
            var vectorConnection = ConnectionManager.BuildConnection(
                $"{_serverURL}/VectorSyncHub", _userName);
            var connectedUserConnection = ConnectionManager.BuildConnection(
                $"{_serverURL}/ConnectedUsersHub", _userName);

            ConnectionManager.StartConnection<string, ConnectedUserInfo>(
                connectedUserConnection,
                "ReceiveConnectedUser",
                "New Connection Info Received:",
                false);

            ConnectionManager.StartConnection<string, string>(
                connectedUserConnection,
                "ReceiveDisconnectedUser",
                "Closed Connection Info Received:");

            List<Task> tasks = new();
            if (_vectorLoop)
            {
                ConnectionManager.StartConnection<string, AvatarVectors>(
                    vectorConnection,
                    "ReceiveVectors",
                    "New Vector Received:");

                foreach (var s in spaces) {
                    tasks.Add(VectorLoopAsync(vectorConnection, s, listenOnly));
                }
                Task.WhenAll(tasks).Wait();
            }
            else
            {
                ConnectionManager.StartConnection<string, string>(
                    chatConnection,
                    "ReceiveMessage",
                    "Chat Message Received:");

                foreach(var s in spaces) {
                    tasks.Add(SayHello(chatConnection, s));
                }
                Task.WhenAll().Wait();
                WaitAndClose(chatConnection).Wait();
            }
        }

        private async Task VectorLoopAsync(
                HubConnection connection, 
                string space,
                bool listenOnly)
        {
            var rand = new Random();
            var playerId = Guid.NewGuid().ToString();

            int count = 0;

            // Subscribe to VectorSyncHub
            await connection.InvokeAsync("SubscribeToSpace", space);
            while(keepRunning)
            {
                // 10 loops to test if subscription is cancelled at the end
                if (count++ > 10)
                    break;

                await Task.Delay(1 * 500);
                if (listenOnly)
                    continue;

                await connection.InvokeAsync("SendVectors", new AvatarVectors()
                {
                    Space = space,
                    PlayerId = playerId,
                    BodyVector = new Vector3(
                        rand.Next(0, 100), 
                        rand.Next(0, 100), 
                        rand.Next(0, 100)),
                    HeadVector = new Vector2(
                        rand.Next(0, 100), 
                        rand.Next(0, 100))
                });
            }

            // Unsubscribe when tasks are completed
            await connection.InvokeAsync("UnsubscribeFromSpace", space);
        }

        private async Task WaitAndClose(HubConnection connection)
        {
            // Console.WriteLine("Waiting in Monitor Loop");
            _quitEvent.WaitOne();
            await connection.StopAsync();
        }

        private async Task SayHello(HubConnection connection, string space)
        {
            // Subscribe to ChatHub
            await connection.InvokeAsync("SubscribeToSpace", space);
            await connection.InvokeAsync("SendMessage", 
            new {
                Message = $"Console Application Started for {_userName}",
                Space = space
            });
            await connection.InvokeAsync("UnsubscribeFromSpace", space);
        }

    }
}