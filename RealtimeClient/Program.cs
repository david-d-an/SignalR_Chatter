using Microsoft.AspNetCore.SignalR.Client;
using RealtimeClient.SignalR.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace RealtimeClient.SignalR
{
    class Program
    {
        /********************************************************/
        /* dotnet RealtimeClient.dll                    */
        /* Argument:                                            */
        /*      [0] Server URL: https://localhost:5001          */
        /*      [1] Loop Indicator: loop                        */
        /*      [2] Listen Only: listen                         */
        /* example:                                             */
        /*   dontnet run https://localhost:5001 loop listen     */
        /********************************************************/

        public static void Main(string[] args)
        {
            var argList = args.ToList();
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet RealtimeClient.dll {ServerUrl} [loop] [listen]");
                return;
            }

            string serverURL = args[0];
            bool vectorLoop = false;
            bool listenOnly = false;

            if (args.Contains("loop"))
                vectorLoop = true;
            if (args.Contains("listen"))
                listenOnly = true;

            string userName = null;
            while (string.IsNullOrWhiteSpace(userName)) {
                Console.Write("Enter Username: ");
                userName = Console.ReadLine();
            }

            HashSet<string> spaces = new();
            string space = null;
            while (true) {
                Console.Write("Enter Space (enter blank when done): ");
                space = Console.ReadLine().ToUpper().Trim();

                if (!string.IsNullOrWhiteSpace(space))
                    spaces.Add(space);
                else if (spaces.Count > 0)
                    break;
            }


            new VectorSyncClient(
                serverURL,
                userName, 
                vectorLoop)
            .Run(spaces, listenOnly);
        }
    }
}
