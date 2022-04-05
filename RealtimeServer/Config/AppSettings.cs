using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace RealtimeServer.SignalR.Config;
public static class AppSettings {
    // Environment is Production unless specified.
    public static readonly string EnvName =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
        "Production";

    public static IConfiguration Config { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{EnvName}.json", true)
        .AddEnvironmentVariables()
        .Build();
}