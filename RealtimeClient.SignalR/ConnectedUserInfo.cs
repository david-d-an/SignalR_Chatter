using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeClient
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

        public override string ToString()
        {
            // return $"({Id}) - ({Name}) - ({Type.ToString()})";
            return ToString();
        }

        private string ToString(string indentation = "  ") {
            string ind = indentation;
            return string.Join(
                Environment.NewLine,
                $"{ind}ConnectionId: {Id}", 
                $"{ind}ConnectionType: {Type.ToString()}"
            );

        }
    }
}
