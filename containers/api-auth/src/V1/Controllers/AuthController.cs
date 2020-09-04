using Auth.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using System.Threading.Tasks;

namespace Auth.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        private readonly IUserAppService _userAppService;

        public AuthController(INotificationHandler<DomainNotification> notifications,
                              IMediatorHandler mediatorHandler,
                              IUserAppService userAppService) : base(notifications, mediatorHandler)
        {
            _userAppService = userAppService;
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var succeeded = await _userAppService.LoginAsync(loginUser);

            if (succeeded)
            {
                return CustomResponse(await _userAppService.ObterJwtAsync(loginUser.Email));
            }

            return CustomResponse();
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var succeeded = await _userAppService.RegistrarAsync(registerUser);

            if (succeeded)
            {
                return CustomResponse(await _userAppService.ObterJwtAsync(registerUser.Email));
            }

            return CustomResponse();
        }
    }
}
