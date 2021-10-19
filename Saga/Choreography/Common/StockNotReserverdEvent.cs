using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StockNotReserverdEvent : IStockNotReserverdEvent
    {
        public StockNotReserverdEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Message { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
