using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.Attributes
{
    public class LanguageFilterAttribute : Attribute

    {
        public int FilterArgument { get; set; }
    }
}
