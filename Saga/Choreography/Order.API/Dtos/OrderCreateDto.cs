using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Dtos
{
    public class OrderCreateDto
    {
        public OrderCreateDto()
        {
            OrderItems = new List<OrderItemDto>();
        }

        public string BuyerId { get; set; }
        public PaymentDto Payment { get; set; }
        public AddressDto Address { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
