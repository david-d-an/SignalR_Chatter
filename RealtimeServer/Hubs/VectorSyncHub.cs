using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealtimeServer.SignalR.Helpers;
using RealtimeServer.SignalR.Services.ConnectedUserService;

namespace RealtimeServer.SignalR.Hubs
{
    public class VectorSyncHub : Hub
    {
        private readonly ILogger<VectorSyncHub> _logger;
        private readonly IHubContext<ConnectedUsersHub> _connectedUsersHub;
        private readonly IConnectedUserService _connectedUserService;

        public string Username { 
            get {
                HttpContext httpContext = Context.GetHttpContext();
                string user = httpContext.Request.Query["username"];
                return user;
            }
        }

        public VectorSyncHub(
            ILogger<VectorSyncHub> logger, 
            IHubContext<ConnectedUsersHub> connectedUsersHub, 
            IConnectedUserService connectedUserService)
        {
            _logger = logger;
            _connectedUsersHub = connectedUsersHub;
            _connectedUserService = connectedUserService;
        }

        public async Task SendVectors(object vectors)
        {
            string space = HubHelper.GetValue(vectors, "space");

            _logger.LogDebug($"Vector Sync for {Username}: {vectors}");

            if (!string.IsNullOrWhiteSpace(space))
            {
                /*********************************************************/
                /* Space value is not to be included per current design  */
                /* To be able to space subscription,, Space needs to be  */
                /* included in Vector, which is confrimed at client side */
                /*********************************************************/
                HubHelper.SetValue(vectors, "Space", space);

                // Send acknowledgement to Group only
                await Clients
                    .Group(space)
                    .SendAsync("ReceiveVectors", Username, vectors);
            }
            else
            {
                Console.WriteLine($"Couldn't identify group.");
                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync("ReceiveVectors", Username, vectors);
            }
        }

        public async Task SubscribeToSpace(string space)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, space);
            _logger.LogInformation($"{Username} subscribed to {space}");
        }

        public async Task UnsubscribeFromSpace(string space) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, space);
            _logger.LogInformation($"{Username} unsubscribed from {space}");
        }

        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();
            string username = httpContext.Request.Query["username"];
            string userIdFormatted = $"user ({username}) with ID {Context.ConnectionId}";
            // string space = httpContext.Request.Query["space"];

            // await Groups.AddToGroupAsync(Context.ConnectionId, space);

            var userInfo = new ConnectedUserInfo
            {
                Id = Context.ConnectionId,
                Name = username,
                Type = ConnectedUserInfo.ConnectionType.VectorSync
            };

            if (_connectedUserService.AddConnectedUserInfo(userInfo))
            {
                _logger.LogInformation(
                    $"New connected {userIdFormatted}");
            }
            else
            {
                _logger.LogError(
                    $"Unable to add new connected {userIdFormatted}");
            }

            await _connectedUsersHub.Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("ReceiveConnectedUser", username, userInfo);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            HttpContext httpContext = Context.GetHttpContext();
            string username = httpContext.Request.Query["username"];
            string userIdFormatted = $"({username}) with ID {Context.ConnectionId}";

            if (_connectedUserService.RemoveConnectedUserInfo(Context.ConnectionId))
            {
                _logger.LogInformation($"User Removed {userIdFormatted}");
            }
            else
            {
                _logger.LogInformation($"Unable to remove {userIdFormatted}");
            }

            _connectedUsersHub.Clients
                .All
                .SendAsync(
                    "ReceiveDisconnectedUser", 
                    username, 
                    ConnectedUserInfo.ConnectionType.VectorSync
                );

            return base.OnDisconnectedAsync(exception);
        }
    }
}
