using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class OrderRequestCompletedEvent : IOrderRequestCompletedEvent
    {
        public OrderRequestCompletedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public int OrderId { get; set; }

        public Guid CorrelationId { get; }
    }
}
