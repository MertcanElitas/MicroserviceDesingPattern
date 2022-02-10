using System.Threading;
using System.Threading.Tasks;
using EventSourceUI.Commands;
using EventSourceUI.EventStores;
using MediatR;

namespace EventSourceUI.Handler
{
    public class ChangeProductNameCommandHandler : IRequestHandler<ChangeProductNameCommand>
    {
        private readonly ProductStream _stream;

        public ChangeProductNameCommandHandler (ProductStream stream)
        {
            _stream = stream;
        }

        public async Task<Unit> Handle(ChangeProductNameCommand request, CancellationToken cancellationToken)
        {
            _stream.NameChanged(request.ChangeProductNameDto);

            await _stream.SaveAsync();
            
            return Unit.Value;
        }
    }
}