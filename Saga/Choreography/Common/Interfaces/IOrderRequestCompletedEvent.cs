using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IOrderRequestCompletedEvent : CorrelatedBy<Guid>
    {
        int OrderId { get; set; }
    }
}
