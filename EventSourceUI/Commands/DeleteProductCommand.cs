using System;
using MediatR;

namespace EventSourceUI.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}