using Common;
using Common.Events;
using Common.Interfaces;
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
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(AppDbContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
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

            var orderCreatedRequestEvent = new OrderCreatedRequestEvent()
            {
                BuyerId = dto.BuyerId,
                OrderId = newOrder.Id,
                PaymentMessage = new PaymentMessage()
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

            var sendProvider = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQConstants.OrderSaga}"));

            await sendProvider.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);

            return Ok();
        }
    }
}
