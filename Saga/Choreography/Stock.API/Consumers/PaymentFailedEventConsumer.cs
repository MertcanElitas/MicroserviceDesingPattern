using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;
        public PaymentFailedEventConsumer(AppDbContext dbContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var data = context.Message;

            foreach (var orderItem in data.OrderItemMessages)
            {
                var model = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                if (model != null)
                {
                    model.Count += orderItem.Count;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Stock was released for Order Id:{data.OrderId}");
                }
            }
        }
    }
}
