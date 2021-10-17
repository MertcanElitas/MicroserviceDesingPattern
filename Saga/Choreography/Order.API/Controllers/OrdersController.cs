using Common;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndPoint;

        public OrdersController(AppDbContext context, IPublishEndpoint publishEndPoint)
        {
            _context = context;
            _publishEndPoint = publishEndPoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            var newOrder = new Models.Order()
            {
                BuyerId = dto.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address()
                {
                    Line = dto.Address.Line,
                    Province = dto.Address.Province,
                    District = dto.Address.District
                },
                Items = dto.OrderItems.Select(x => new OrderItem()
                {
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Count = x.Count
                }).ToList()
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = dto.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage()
                {
                    CardName = dto.Payment.CardName,
                    CardNumber = dto.Payment.CardNumber,
                    Expiration = dto.Payment.Expiration,
                    CVV = dto.Payment.CVV,
                    TotalPrice = dto.OrderItems.Sum(x => x.Price * x.Count),
                },
                OrderItems = dto.OrderItems.Select(x => new OrderItemMessage()
                {
                    Count = x.Count,
                    ProductId = x.ProductId
                }).ToList()
            };

            await _publishEndPoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
