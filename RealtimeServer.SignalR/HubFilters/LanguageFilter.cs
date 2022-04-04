using Microsoft.AspNetCore.SignalR;
using RealtimeServer.SignalR.Attributes;
using RealtimeServer.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeServer.SignalR.HubFilters
{
    public class LanguageFilter : IHubFilter
    {
        // populated from a file or inline
        private List<string> bannedPhrases = new List<string> { "async void", ".Result", "curseword :)" };

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            var languageFilter = (LanguageFilterAttribute)Attribute.GetCustomAttribute(
                invocationContext.HubMethod, typeof(LanguageFilterAttribute));
            if (languageFilter != null &&
                invocationContext.HubMethodArguments.Count > languageFilter.FilterArgument &&
                HubHelper.GetValue(invocationContext.HubMethodArguments[languageFilter.FilterArgument], "Message") is string str)
            {
                foreach (var bannedPhrase in bannedPhrases)
                {
                    str = str.Replace(bannedPhrase, "***", true, CultureInfo.InvariantCulture);
                }

                var arguments = invocationContext.HubMethodArguments.ToArray();
                HubHelper.SetValue(arguments[languageFilter.FilterArgument], "Message", str);
                invocationContext = new HubInvocationContext(invocationContext.Context,
                    invocationContext.ServiceProvider,
                    invocationContext.Hub,
                    invocationContext.HubMethod,
                    arguments);
            }

            return await next(invocationContext);
        }
    }
}
