using RealtimeServer.SignalR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.Services.ConnectedUserService
{
    public interface IConnectedUserService
    {
        List<ConnectedUserInfo> Connections { get; }
        bool AddConnectedUserInfo(ConnectedUserInfo userInfo);
        bool RemoveConnectedUserInfo(string Id);
    }
}
