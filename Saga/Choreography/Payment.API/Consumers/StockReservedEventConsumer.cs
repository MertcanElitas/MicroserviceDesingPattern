using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReserverdEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReserverdEvent> context)
        {
            var balance = 3000m;

            var data = context.Message;

            if (balance >= data.Payment.TotalPrice)
            {
                _logger.LogInformation($"{data.Payment.TotalPrice} TL was withdraw from credit card for user id={data.BuyerId}");

                var paymentSuccessedEvent = new PaymentSuccessedEvent()
                {
                    BuyerId = data.BuyerId,
                    OrderId = data.OrderId
                };

                await _publishEndpoint.Publish(paymentSuccessedEvent);
            }
            else
            {
                _logger.LogInformation($"{data.Payment.TotalPrice} TL was not  withdraw from credit card for user id={data.BuyerId}");

                var publishFailedEvent = new PaymentFailedEvent()
                {
                    BuyerId = data.BuyerId,
                    OrderId = data.OrderId,
                    Message = "not enough balance",
                    OrderItemMessages = data.OrderItemMessages
                };

                await _publishEndpoint.Publish(publishFailedEvent);
            }

        }
    }
}
