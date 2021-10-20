using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IOrderRequestFailedEvent : CorrelatedBy<Guid>
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
}
