using EventSourceUI.Dtos;
using MediatR;

namespace EventSourceUI.Commands
{
    public class ChangeProductPriceCommand : IRequest
    {
        public ChangeProductPriceDto ChangeProductPriceDto { get; set; }
    }
}