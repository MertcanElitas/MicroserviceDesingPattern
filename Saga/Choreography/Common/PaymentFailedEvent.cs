using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PaymentFailedEvent
    {
        public PaymentFailedEvent()
        {
            OrderItemMessages = new List<OrderItemMessage>();
        }

        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string Message { get; set; }

        public List<OrderItemMessage> OrderItemMessages { get; set; }
    }
}
