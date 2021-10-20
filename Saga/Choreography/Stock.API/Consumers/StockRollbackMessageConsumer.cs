using Common.Message;
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
    public class StockRollbackMessageConsumer : IConsumer<IStockRollBackMessage>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockRollbackMessageConsumer> _logger;

        public StockRollbackMessageConsumer(AppDbContext dbContext, ILogger<StockRollbackMessageConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStockRollBackMessage> context)
        {
            var data = context.Message;

            foreach (var orderItem in data.OrderItems)
            {
                var model = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                if (model != null)
                {
                    model.Count += orderItem.Count;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Stock was released");
                }
            }
        }
    }
}
