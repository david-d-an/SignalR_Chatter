using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using RealtimeServer.SignalR.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.HubFilters
{
    public class SpaceFilter : IHubFilter
    {
        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            HttpContext httpContext = context.Context.GetHttpContext();

            string space = httpContext.Request.Query["space"];

            if (string.IsNullOrWhiteSpace(space))
            {
                throw new ArgumentNullException("Space must be passed with Query Parameters");
            }


            return next(context);
        }
    }
}
