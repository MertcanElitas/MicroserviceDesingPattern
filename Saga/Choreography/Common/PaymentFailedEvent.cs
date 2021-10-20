﻿using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public PaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
            OrderItemMessages = new List<OrderItemMessage>();
        }


        public string Reason { get; set; }

        public List<OrderItemMessage> OrderItemMessages { get; set; }

        public Guid CorrelationId { get; }
    }
}
