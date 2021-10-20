using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StockReservedPaymentRequest : IStockReservedRequestPayment
    {
        public StockReservedPaymentRequest(Guid correlationId)
        {
            CorrelationId = correlationId;
            OrderItemMessages = new List<OrderItemMessage>();
        }

        public string BuyerId { get; set; }
        public Guid CorrelationId { get; set; }

        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItemMessages { get; set; }
    }
}
