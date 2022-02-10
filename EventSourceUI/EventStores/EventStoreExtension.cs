using System;
using System.Diagnostics;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSourceUI.EventStores
{
    public static class EventStoreExtension
    {
        public static void AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            IEventStoreConnection connection = EventStoreConnection.Create(
                connectionString: configuration.GetConnectionString("EventStore"),
                connectionName: "Mertcan_App",
                builder: ConnectionSettings.Create().KeepReconnecting()
            );
            
            connection.ConnectAsync().Wait();

            services.AddSingleton(connection);

            var logFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });
           
            var logger = logFactory.CreateLogger("Startup");
            
            connection.Connected +=
                ( sender,  args) =>
                {
                    Console.WriteLine("Connection successful");
                };

            connection.ErrorOccurred += ( sender,  args) =>
            {
                logger.LogError(args.Exception.Message);
                Console.WriteLine("Connection closed");
            } ;
        }
    }
}