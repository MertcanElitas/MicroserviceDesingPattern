using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
    {
        public OrderCreatedRequestEvent()
        {
            OrderItems = new List<OrderItemMessage>();
        }

        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
        public PaymentMessage PaymentMessage { get; set; }
    }
}
