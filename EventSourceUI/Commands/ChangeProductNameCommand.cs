using EventSourceUI.Dtos;
using MediatR;

namespace EventSourceUI.Commands
{
    public class ChangeProductNameCommand : IRequest
    {
        public ChangeProductNameDto ChangeProductNameDto { get; set; }
    }
}