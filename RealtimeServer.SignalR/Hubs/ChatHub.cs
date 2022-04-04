using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealtimeServer.SignalR.Attributes;
using RealtimeServer.SignalR.Helpers;
using RealtimeServer.SignalR.Services.ConnectedUserService;

namespace RealtimeServer.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IHubContext<ConnectedUsersHub> _connectedUsersHub;
        private readonly IConnectedUserService _connectedUserService;

        private string Username {
            get {
                HttpContext httpContext = Context.GetHttpContext();
                string username = httpContext.Request.Query["username"];
                return username;
            }
        }

        public ChatHub(
            ILogger<ChatHub> logger, 
            IHubContext<ConnectedUsersHub> connectedUsersHub, 
            IConnectedUserService connectedUserService)
        {
            _logger = logger;
            _connectedUsersHub = connectedUsersHub;
            _connectedUserService = connectedUserService;
        }

        [LanguageFilter(FilterArgument = 0)]
        public async Task SendMessage(object messageObject)
        {
            HttpContext httpContext = Context.GetHttpContext();
            string user = httpContext.Request.Query["username"];
            string space = HubHelper.GetValue(messageObject, "space");
            string message = HubHelper.GetValue(messageObject, "message");

            if (space != null)
            {
                // Send message to group only
                await Clients
                    .Group(space)
                    .SendAsync("ReceiveMessage", 
                        user,
                        $"Message: {message}",
                        space);
            }
            else
            {
                Console.WriteLine($"Couldn't identify group.");
                await Clients
                    .All
                    .SendAsync("ReceiveMessage", 
                        user, 
                        $"Message: {message}");
            }
        }

        public async Task SubscribeToSpace(string space) {
            await Groups.AddToGroupAsync(Context.ConnectionId, space);
            _logger.LogInformation($"{Username} subscribed to {space}");
        }

        public async Task UnsubscribeFromSpace(string space) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, space);
            _logger.LogInformation($"{Username} unsubscribed from {space}");
        }

        public override async Task OnConnectedAsync()
        {
            string userIdFormatted = $"user ({Username}) with ID {Context.ConnectionId}";

            var userInfo = new ConnectedUserInfo
            {
                Id = Context.ConnectionId,
                Name = Username,
                Type = ConnectedUserInfo.ConnectionType.Chat
            };

            if (_connectedUserService.AddConnectedUserInfo(userInfo))
            {
                _logger.LogInformation($"New connected {userIdFormatted}");
            }
            else
            {
                _logger.LogError($"Unable to add new connected {userIdFormatted}");
            }

            await _connectedUsersHub.Clients
                .AllExcept(Context.ConnectionId)
                .SendAsync("ReceiveConnectedUser", Username, userInfo);

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
                    ConnectedUserInfo.ConnectionType.Chat);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
