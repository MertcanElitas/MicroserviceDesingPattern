using EventSourceUI.Dtos;
using MediatR;

namespace EventSourceUI.Commands
{
    public class CreateProductCommand : IRequest
    {
        public CreateProductDto CreateProductDto { get; set; }
    }
}