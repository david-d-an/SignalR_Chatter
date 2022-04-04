using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeServer.SignalR.HubFilters;
using RealtimeServer.SignalR.Hubs;
using RealtimeServer.SignalR.Services.ConnectedUserService;
using RealtimeServer.SignalR.Logger;
using Serilog;
using StackExchange.Redis;

namespace RealtimeServer.SignalR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = CustomLogger.Logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string redisConnStr = 
                Configuration.GetConnectionString("RedisConnection");

            services.AddRazorPages();

            var signalrService = services
                .AddSignalR(options =>
                {
                    //Filters can be added here to apply to ALL Hubs
                })
                .AddHubOptions<ChatHub>(options => {
                    options.AddFilter<LanguageFilter>();
                })
                .AddMessagePackProtocol();

            // Redis backplane is used only if connection stirng is provided
            // in appSettings.json files.
            if (string.IsNullOrWhiteSpace(redisConnStr)) {
                Log.Information("Redis backplane is not available.");
            } else {
                Log.Information("Redis backplane activated.");
                Log.Information($"Redis Connection: {redisConnStr}");

                signalrService
                .AddStackExchangeRedis(options => {
                    options.Configuration.ChannelPrefix = 
                        "Virbela.VNext.RealtimeServer";

                    options.ConnectionFactory = async writer => {
                        var config = new ConfigurationOptions {
                            AbortOnConnectFail = false
                        };
                        config.EndPoints.Add(redisConnStr);
                        config.SetDefaultPorts();
                        var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                        connection.ConnectionFailed += (_, e) => {
                            Log.Error("Connection to Redis failed.");
                        };

                        if (!connection.IsConnected) {
                            Log.Error("Did not connect to Redis.");
                        }

                        return connection;
                    };
                });
            }

            services.AddScoped<IConnectedUserService, StaticConnectedUserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<VectorSyncHub>("/vectorSyncHub");
                endpoints.MapHub<ConnectedUsersHub>("/connectedUsersHub");
            });
        }
    }
}

