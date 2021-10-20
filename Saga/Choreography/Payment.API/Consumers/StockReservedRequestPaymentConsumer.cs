using Common;
using Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.API.Consumers
{
    public class StockReservedRequestPaymentConsumer : IConsumer<IStockReservedRequestPayment>
    {
        private readonly ILogger<StockReservedRequestPaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedRequestPaymentConsumer(ILogger<StockReservedRequestPaymentConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IStockReservedRequestPayment> context)
        {
            var balance = 3000m;

            var data = context.Message;

            if (balance >= data.Payment.TotalPrice)
            {
                _logger.LogInformation($"{data.Payment.TotalPrice} TL was withdraw from credit card for user id={data.BuyerId}");

                var paymentSuccessedEvent = new PaymentSuccessedEvent(data.CorrelationId);

                await _publishEndpoint.Publish(paymentSuccessedEvent);
            }
            else
            {
                _logger.LogInformation($"{data.Payment.TotalPrice} TL was not  withdraw from credit card for user id={data.BuyerId}");

                var publishFailedEvent = new PaymentFailedEvent(data.CorrelationId)
                {
                    Reason = "not enough balance",
                    OrderItemMessages = data.OrderItemMessages
                };

                await _publishEndpoint.Publish(publishFailedEvent);
            }
        }
    }
}
