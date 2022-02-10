using System.Threading;
using System.Threading.Tasks;
using EventSourceUI.Commands;
using EventSourceUI.EventStores;
using EventStore.ClientAPI.Exceptions;
using MediatR;

namespace EventSourceUI.Handler
{
    public class DeleteProductCommandHandler:IRequestHandler<DeleteProductCommand>
    {
        private readonly ProductStream _stream;

        public DeleteProductCommandHandler (ProductStream stream)
        {
            _stream = stream;
        }
        
        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _stream.Delete(request.Id);

            await _stream.SaveAsync();
            
            return Unit.Value;
        }
    }
}