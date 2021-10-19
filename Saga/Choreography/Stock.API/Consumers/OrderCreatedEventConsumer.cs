using Common;
using Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpoint;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(AppDbContext dbContext,
             IPublishEndpoint publishEndpoint,
             ISendEndpointProvider sendEndpoint,
             ILogger<OrderCreatedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
            _sendEndpoint = sendEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var data = context.Message;

            var isStockCountEnough = true;

            var stockData = await _dbContext.Stocks.ToListAsync();

            foreach (var orderItem in data.OrderItems)
            {
                isStockCountEnough = stockData.Any(x => x.ProductId == orderItem.ProductId && x.Count >= orderItem.Count);

                if (!isStockCountEnough)
                    break;
            }

            if (isStockCountEnough)
            {
                foreach (var orderItem in data.OrderItems)
                {
                    var stockModel = stockData.FirstOrDefault(x => x.ProductId == orderItem.ProductId);
                    stockModel.Count -= orderItem.Count;
                }

                await _dbContext.SaveChangesAsync();

                var sendEndPoint = await _sendEndpoint.GetSendEndpoint(new Uri($"queue:{RabbitMQConstants.StockReserverdEventQueueName}"));

                StockReserverdEvent stockReserverd = new StockReserverdEvent(context.Message.CorrelationId)
                {
                    OrderItemMessages = data.OrderItems
                };

                await _publishEndpoint.Publish(stockReserverd);
            }
            else
            {
                var stockNotReserved = new StockNotReserverdEvent(context.Message.CorrelationId)
                {
                    Message = $"Not enough stock"
                };

                await _publishEndpoint.Publish(stockNotReserved);

                _logger.LogInformation($"Not enough stock for OrderId:{context.Message.CorrelationId}");
            }
        }
    }
}
