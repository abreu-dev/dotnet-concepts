using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using System.Linq;

namespace Auth.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _mediatorHandler;

        protected MainController(INotificationHandler<DomainNotification> notifications,
                                 IMediatorHandler mediatorHandler)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediatorHandler = mediatorHandler;
        }

        protected bool ValidOperation()
        {
            return !_notifications.HasNotifications();
        }

        protected async void NotifyError(string key, string mensagem)
        {
            await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification(key, mensagem));
        }

        protected void NotifyErrorsInvalidModel(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                var message = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                NotifyError("ModelState", message);
            }
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (ValidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notifications.GetNotifications().Select(n => n.Value)
            });
        }
    }
}
