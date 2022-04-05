using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.Helpers
{
    public class ConnectedUserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ConnectionType Type { get; set; }


        public enum ConnectionType
        {
            VectorSync,
            Chat,
            ConnectedUsers
        }
    }
}
