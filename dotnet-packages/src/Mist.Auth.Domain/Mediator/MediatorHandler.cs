using MediatR;
using Mist.Auth.Domain.Notifications;
using System.Threading.Tasks;

namespace Mist.Auth.Domain.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task RaiseDomainNotificationAsync<T>(T notification) where T : DomainNotification
        {
            await _mediator.Publish(notification);
        }
    }
}
