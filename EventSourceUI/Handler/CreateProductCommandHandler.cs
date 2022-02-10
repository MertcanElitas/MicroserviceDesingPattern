using System.Threading;
using System.Threading.Tasks;
using EventSourceUI.Commands;
using EventSourceUI.EventStores;
using MediatR;

namespace EventSourceUI.Handler
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        private readonly  ProductStream _stream;

        public CreateProductCommandHandler (ProductStream stream)
        {
            _stream = stream;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _stream.Created(request.CreateProductDto);

            await _stream.SaveAsync();
            
            return Unit.Value;
        }
    }
}