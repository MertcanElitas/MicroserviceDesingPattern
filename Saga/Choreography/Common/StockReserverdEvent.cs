using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StockReserverdEvent : IStockReservedEvent
    {
        public StockReserverdEvent(Guid correlationId)
        {
            OrderItemMessages = new List<OrderItemMessage>();
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItemMessages { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
