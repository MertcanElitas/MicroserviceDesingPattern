using System;
using EventSource.Shared.Events;
using EventSourceUI.Dtos;
using EventStore.ClientAPI;

namespace EventSourceUI.EventStores
{
    public class ProductStream : AbstractStream
    {
        public static string StreamName => "dstream";
        public static string GroupName => "dgroup";

        public ProductStream(IEventStoreConnection eventStoreConnection) : base(eventStoreConnection,
            StreamName)
        {
        }

        public void Created(CreateProductDto productDto)
        {
            Events.Add(new ProductCreatedEvent()
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Price = productDto.Price,
                Stock = productDto.Stock,
                UserId = productDto.UserId
            });
        }

        public void NameChanged(ChangeProductNameDto changeProductNameDto)
        {
            Events.Add(new ProductNameChangedEvent()
            {
                Id = Guid.NewGuid(),
                ChangedName = changeProductNameDto.Name
            });
        }

        public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
        {
            Events.Add(new ProductPriceChangedEvent()
            {
                Id = Guid.NewGuid(),
                ChangedPrice = changeProductPriceDto.Price
            });
        }

        public void Delete(Guid id)
        {
            Events.Add(new ProductDeletedEvent()
            {
                Id = id
            });
        }
    }
}