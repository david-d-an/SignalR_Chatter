using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeServer.SignalR.Hubs
{
    public static class HubHelper {

        /**********************************************/
        /* Example of Key                             */
        /*   SpaceId, WordId, Message, UserName, etc. */
        /**********************************************/
        public static string GetValue(object vectors, string key)
        {

            Dictionary<object, object> objectDictionary = 
                vectors as Dictionary<object, object>;

            //There is nothing in the data that matches
            if (objectDictionary == null)
                return null;

            return objectDictionary.SingleOrDefault(k => 
                key.Equals( 
                    k.Key as string, 
                    StringComparison.InvariantCultureIgnoreCase))
                .Value as string;
        }

        public static void SetValue(object vectors, string key, string value)
        {
            Dictionary<object, object> objectDictionary = 
                vectors as Dictionary<object, object>;

            //There is nothing in the data that matches
            if (objectDictionary == null)
                return;

            objectDictionary[key] = value;
        }
    }
}
