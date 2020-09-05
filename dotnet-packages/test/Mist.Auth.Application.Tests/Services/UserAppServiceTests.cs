using FluentAssertions;
using Microsoft.Extensions.Options;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.Services;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Commands.UserCommands;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Configuration;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Mist.Auth.Application.Tests.Services
{
    public class UserAppServiceTests
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSettings> _appSettings;

        private readonly IUserAppService _userAppService;

        public UserAppServiceTests()
        {
            _mediatorHandler = Substitute.For<IMediatorHandler>();
            _userRepository = Substitute.For<IUserRepository>();

            var appSettings = new AppSettings()
            {
                Secret = "TESTETESTETESTETESTE",
                Expires = 1,
                Issuer = "TESTE",
                Audience = "TESTE"
            };
            _appSettings = Options.Create(appSettings);

            _userAppService = new UserAppService(_mediatorHandler, _userRepository, _appSettings);
        }

        [Fact]
        public async Task LoginDevePassar()
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

            var response = await _userAppService.LoginAsync(loginViewModel);

            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Errors.Should().BeNull();
        }

        [Fact]
        public async Task LoginDeveFalhar()
        {
            var loginViewModel = new LoginUserViewModel()
            {
                Email = "example@example.com",
                Password = "123456"
            };

            _userRepository.AuthenticateAsync(loginViewModel.Email, loginViewModel.Password).Returns(false);

            var response = await _userAppService.LoginAsync(loginViewModel);

            response.Success.Should().BeFalse();
            response.Errors.Should().BeEquivalentTo(new List<string> { "Invalid email or password." });
            response.Data.Should().BeNull();
        }

        [Fact] 
        public async Task RegistroDeveFalharFormatoEmail()
        {
            var registerViewModel = new RegisterUserViewModel()
            {
                Email = "example",
                Password = "123456"
            };

            _userRepository.GetAllAsync().Returns(new List<User>());

            await _userAppService.RegisterAsync(registerViewModel);

            var command = new RegisterUserCommand()
            {
                Entity = new User
                {
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password
                }
            };

            await _mediatorHandler
                    .Received(1)
                    .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                        dm.Key == command.MessageType && dm.Value == "Invalid email format."));
        }

        [Fact]
        public async Task RegistroDevePassar()
        {
            var registerViewModel = new RegisterUserViewModel()
            {
                Email = "example@example.com",
                Password = "123456"
            };

            _userRepository.GetAllAsync().Returns(new List<User>());

            await _userAppService.RegisterAsync(registerViewModel);

            await _mediatorHandler.DidNotReceive().RaiseDomainNotificationAsync(Arg.Any<DomainNotification>());
        }
    }
}
