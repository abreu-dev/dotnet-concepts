﻿using Auth.Api.Controllers;
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

            var response = await _userAppService.LoginAsync(loginUser);

            if (response.Success)
            {
                return Ok(new
                {
                    success = true,
                    data = response.Data
                });
            }

            return Unauthorized(new
            {
                success = false,
                errors = response.Errors
            });
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _userAppService.RegistrarAsync(registerUser);

            return CustomResponse();
        }
    }
}
