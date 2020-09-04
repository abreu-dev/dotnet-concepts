using Mist.Auth.Domain.Notifications;
using System.Threading.Tasks;

namespace Mist.Auth.Domain.Mediator
{
    public interface IMediatorHandler
    {
        Task RaiseDomainNotificationAsync<T>(T notification) where T : DomainNotification;
    }
}
