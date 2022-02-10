using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventSource.Shared.Events;
using EventStore.ClientAPI;

namespace EventSourceUI.EventStores
{
    public abstract class AbstractStream
    {
        protected readonly List<IEvent> Events = new List<IEvent>();
        private string _streamName { get; }
        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(IEventStoreConnection eventStoreConnection, string streamName)
        {
            _eventStoreConnection = eventStoreConnection;
            _streamName = streamName;
        }

        public async Task SaveAsync()
        {
            var newEvents = Events.Select(x => new EventData(
                Guid.NewGuid(),
                x.GetType().Name,
                true,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, x.GetType())),
                Encoding.UTF8.GetBytes(x.GetType().FullName)
            )).ToList();

            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);

            Events.Clear();
        }
    }
}