using Auth.Api.V1.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.Services;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Configuration;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Mist.Auth.Controller.Tests.V1
{
    public class AuthControllerTests
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IUserAppService _userAppService;

        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mediatorHandler = Substitute.For<IMediatorHandler>();
            _userRepository = Substitute.For<IUserRepository>();
            var appSettings = new AppSettings()
            {
                Secret = "TESTETESTETESTETESTE",
                ExpiracaoHoras = 1,
                Emissor = "TESTE",
                ValidoEm = "TESTE"
            };
            _appSettings = Options.Create(appSettings);
            _userAppService = new UserAppService(_mediatorHandler, _userRepository, _appSettings);
            _notifications = Substitute.For<DomainNotificationHandler>();

            _authController = new AuthController(_notifications, _mediatorHandler, _userAppService);
        }

        [Fact]
        public async Task DeveRetornarOk()
        {
            var loginViewModel = new LoginUserViewModel()
            {
                Email = "example@example.com",
                Password = "123456"
            };

            _userRepository.AuthenticateAsync(loginViewModel.Email, loginViewModel.Password).Returns(true);
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = loginViewModel.Email,
                Password = loginViewModel.Password
            };
            _userRepository.FindByEmailAsync(loginViewModel.Email).Returns(user);

            var actionResult = await _authController.Login(loginViewModel);
            actionResult.Should().BeOfType(typeof(OkObjectResult));

            var content = actionResult as OkObjectResult;
            content.Should().NotBeNull();
            content.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task DeveRetornarUnauthorized()
        {
            var loginViewModel = new LoginUserViewModel()
            {
                Email = "example@example.com",
                Password = "123456"
            };

            _userRepository.AuthenticateAsync(loginViewModel.Email, loginViewModel.Password).Returns(false);

            var actionResult = await _authController.Login(loginViewModel);
            actionResult.Should().BeOfType(typeof(UnauthorizedObjectResult));

            var content = actionResult as UnauthorizedObjectResult;
            content.Should().NotBeNull();
            content.Value.Should().NotBeNull();
        }
    }
}
