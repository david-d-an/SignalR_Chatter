using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace RealtimeServer.SignalR.Logger
{
    public static class CustomLogger
    {
        // Environment is Production unless specified.
        private static readonly string envName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            "Production";

        // Environment precedence: Prod > Speified Env name > Development
        public static IConfiguration LogConfig { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{envName}.json", true)
            .AddJsonFile($"appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        public static Serilog.Core.Logger Logger { get; } = new LoggerConfiguration()
            .ReadFrom.Configuration(LogConfig)
            .Enrich.FromLogContext()
            .CreateLogger();
    }
}