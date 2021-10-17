﻿using Common;
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
    public class StockNotReservedEventConsumer : IConsumer<StockNotReserverdEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockNotReservedEventConsumer> _logger;

        public StockNotReservedEventConsumer(AppDbContext dbContext, ILogger<StockNotReservedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReserverdEvent> context)
        {
            var data = context.Message;

            var orderModel = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == data.OrderId);

            if (orderModel != null)
            {
                orderModel.Status = OrderStatus.Fail;
                orderModel.FailMessage = data.Message;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={data.OrderId}) status changed fail :{ orderModel.Status}");
            }
            else
                _logger.LogError($"Order (Id={data.OrderId}) not found");
        }
    }
}
