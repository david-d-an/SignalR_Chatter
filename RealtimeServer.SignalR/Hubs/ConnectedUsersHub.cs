using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealtimeServer.SignalR.Helpers;
using RealtimeServer.SignalR.Services.ConnectedUserService;

namespace RealtimeServer.SignalR.Hubs
{
    public class ConnectedUsersHub : Hub
    {
        private readonly ILogger<ConnectedUsersHub> _logger;
        private readonly IConnectedUserService _connectedUserService;

        public ConnectedUsersHub(
            ILogger<ConnectedUsersHub> logger, 
            IConnectedUserService connectedUserService)
        {
            _logger = logger;
            _connectedUserService = connectedUserService;
        }

        //public async Task SendConnectedUser(string user, ConnectedUserInfo connectedUserInfo)
        //{
        //    _logger.LogDebug($"SendConnectedUser for {user}: {connectedUserInfo}");
        //    await Clients.All.SendAsync("ReceiveConnectedUser", user, connectedUserInfo);
        //}

        public override Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();
            string username = httpContext.Request.Query["username"];
            string userIdFormatted = $"user ({username}) with ID {Context.ConnectionId}";

            var userInfo = new ConnectedUserInfo
            {
                Id = Context.ConnectionId,
                Name = username,
                Type = ConnectedUserInfo.ConnectionType.ConnectedUsers
            };

            if (_connectedUserService.AddConnectedUserInfo(userInfo))
            {
                _logger.LogInformation($"New connected {userIdFormatted}");
            }
            else
            {
                _logger.LogError($"Unable to add new connected {userIdFormatted}");
            }

            foreach (ConnectedUserInfo item in _connectedUserService.Connections)
            {
                Clients
                    .Caller
                    .SendAsync("ReceiveConnectedUser", item.Name, item);
            }

            Clients
                .Others
                .SendAsync("ReceiveConnectedUser", username, userInfo);

            return base.OnConnectedAsync();
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

            Clients
                .All
                .SendAsync(
                    "ReceiveDisconnectedUser", 
                    username, 
                    ConnectedUserInfo.ConnectionType.ConnectedUsers);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
