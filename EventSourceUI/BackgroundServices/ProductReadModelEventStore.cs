using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSource.Shared.Events;
using EventSourceUI.EventStores;
using EventSourceUI.Models;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourceUI.BackgroundServices
{
    public class ProductReadModelEventStore : BackgroundService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadModelEventStore> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ProductReadModelEventStore(IEventStoreConnection eventStoreConnection,
            ILogger<ProductReadModelEventStore> logger, IServiceProvider serviceProvider)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName,
                ProductStream.GroupName, EventAppeared);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            _logger.LogInformation("The message processing...");
            
            var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)}, EventSource.Shared");

            var eventData = Encoding.UTF8.GetString(arg2.Event.Data);

            var @event = JsonSerializer.Deserialize(eventData, type);

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Product product = null;

                switch (@event)
                {
                    case ProductCreatedEvent productCreatedEvent:
                        product = new Product()
                        {
                            Name = productCreatedEvent.Name,
                            Id = productCreatedEvent.Id,
                            Price = productCreatedEvent.Price,
                            Stock = productCreatedEvent.Stock,
                            UserId = productCreatedEvent.UserId
                        };
                        context.Products.Add(product);
                        break;
                    case ProductNameChangedEvent productNameChangedEvent:
                        product = context.Products.FirstOrDefault(x => x.Id == productNameChangedEvent.Id);
                        if (product != null)
                            product.Name = productNameChangedEvent.ChangedName;
                        break;
                    case ProductPriceChangedEvent productPriceChangedEvent:
                        product = context.Products.FirstOrDefault(x => x.Id == productPriceChangedEvent.Id);
                        if (product != null)
                            product.Price = productPriceChangedEvent.ChangedPrice;
                        break;
                    case ProductDeletedEvent productDeletedEvent:
                        product = context.Products.FirstOrDefault(x => x.Id == productDeletedEvent.Id);
                        if (product != null)
                            context.Products.Remove(product);
                        break;
                }

                await context.SaveChangesAsync();
            }
            
            arg1.Acknowledge(arg2.Event.EventId);
        }
    }
}