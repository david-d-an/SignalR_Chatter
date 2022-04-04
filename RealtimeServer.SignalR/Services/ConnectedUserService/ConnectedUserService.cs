using Microsoft.Extensions.Logging;
using RealtimeServer.SignalR.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.Services.ConnectedUserService
{
    public class StaticConnectedUserService : IConnectedUserService
    {

        private static ConcurrentDictionary<string, ConnectedUserInfo> ConnectedUsers = new ConcurrentDictionary<string, ConnectedUserInfo>();

        public List<ConnectedUserInfo> Connections => ConnectedUsers.Select(c => c.Value).ToList();

        public bool AddConnectedUserInfo(ConnectedUserInfo userInfo)
        {
            return ConnectedUsers.TryAdd(userInfo.Id, userInfo);
        }

        public bool RemoveConnectedUserInfo(string Id)
        {
            return ConnectedUsers.TryRemove(Id, out _);
        }

        
    }
}
