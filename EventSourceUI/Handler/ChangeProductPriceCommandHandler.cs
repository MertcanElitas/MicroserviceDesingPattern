using System.Threading;
using System.Threading.Tasks;
using EventSourceUI.Commands;
using EventSourceUI.EventStores;
using MediatR;

namespace EventSourceUI.Handler
{
    public class ChangeProductPriceCommandHandler : IRequestHandler<ChangeProductPriceCommand>
    {
        private readonly ProductStream _stream;

        public ChangeProductPriceCommandHandler (ProductStream stream)
        {
            _stream = stream;
        }

        public async Task<Unit> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
        {
            _stream.PriceChanged(request.ChangeProductPriceDto);

            await _stream.SaveAsync();
            
            return Unit.Value;
        }
    }
}