using System;

namespace EventSource.Shared.Events
{
    public class ProductDeletedEvent:IEvent
    {
        public Guid Id { get; set; }
    }
}