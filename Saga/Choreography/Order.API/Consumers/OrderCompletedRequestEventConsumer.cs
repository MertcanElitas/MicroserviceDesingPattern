using Common;
using Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Consumers
{
    public class OrderCompletedRequestEventConsumer : IConsumer<IOrderRequestCompletedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCompletedRequestEventConsumer> _logger;

        public OrderCompletedRequestEventConsumer(AppDbContext dbContext, ILogger<OrderCompletedRequestEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderRequestCompletedEvent> context)
        {
            var data = context.Message;

            var orderModel = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == data.OrderId);

            if (orderModel != null)
            {
                orderModel.Status = OrderStatus.Complete;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={data.OrderId}) status changed completed :{ orderModel.Status}");
            }
            else
                _logger.LogError($"Order (Id={data.OrderId}) not found");
        }
    }
}
