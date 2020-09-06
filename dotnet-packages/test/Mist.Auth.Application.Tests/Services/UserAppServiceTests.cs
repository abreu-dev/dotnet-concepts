using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Mist.Auth.Application.AutoMapper;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.Services;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Data.Common;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
        private readonly IMapper _mapper;
        private readonly IOptions<AppSettings> _appSettings;

        private readonly IUserAppService _userAppService;

        public UserAppServiceTests()
        {
            _mediatorHandler = Substitute.For<IMediatorHandler>();
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ViewModelToDomainMappingProfile>()).CreateMapper();

            var appSettings = new AppSettings()
            {
                Secret = "UserAppServiceTest",
                Expires = 1,
                Issuer = "UserAppServiceTest",
                Audience = "UserAppServiceTest"
            };
            _appSettings = Options.Create(appSettings);

            _userAppService = new UserAppService(_mediatorHandler, _userRepository, _mapper, _appSettings);
        }

        [Fact]
        public async Task GivenRegisterAsync_WhenEmptyEmail_ThenRaiseDomainNotification()
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "",
                Password = "test123"
            };

            await _userAppService.RegisterAsync(registerUserViewModel);

            await _mediatorHandler
                .Received(1)
                .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                    dm.Key == "RegisterUserCommand" && dm.Value == "Email required."));
        }

        [Fact]
        public async Task GivenRegisterAsync_WhenInvalidEmail_ThenRaiseDomainNotification()
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "test.com",
                Password = "test123"
            };

            await _userAppService.RegisterAsync(registerUserViewModel);

            await _mediatorHandler
                .Received(1)
                .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                    dm.Key == "RegisterUserCommand" && dm.Value == "Invalid email format."));
        }

        [Fact]
        public async Task GivenRegisterAsync_WhenEmptyPassword_ThenRaiseDomainNotification()
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "test@teste.com",
                Password = ""
            };

            await _userAppService.RegisterAsync(registerUserViewModel);

            await _mediatorHandler
                .Received(1)
                .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                    dm.Key == "RegisterUserCommand" && dm.Value == "Password required."));
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("1234567891234567")]
        public async Task GivenRegisterAsync_WhenPasswordIsNotBetweenFiveAndFifteenCharacters_ThenRaiseDomainNotification(string password)
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "test@teste.com",
                Password = password
            };

            await _userAppService.RegisterAsync(registerUserViewModel);

            await _mediatorHandler
                .Received(1)
                .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                    dm.Key == "RegisterUserCommand" && dm.Value == "Password must be between 5 and 15 characters."));
        }

        [Fact]
        public async Task GivenRegisterAsync_WhenEmailAlreadyInUse_ThenRaiseDomainNotification()
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "test@test.com",
                Password = "test123"
            };

            _userRepository.GetAllAsync().Returns(new List<User> { new User() { Email = registerUserViewModel.Email } });
            await _userAppService.RegisterAsync(registerUserViewModel);

            await _mediatorHandler
                .Received(1)
                .RaiseDomainNotificationAsync(Arg.Is<DomainNotification>(dm =>
                    dm.Key == "RegisterUserCommand" && dm.Value == "Email is already in use."));
        }

        [Fact]
        public async Task GivenRegisterAsync_WhenThereIsNoErrors_ThenCallRepositoryAddAsync()
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                Email = "test@test.com",
                Password = "test123"
            };

            _userRepository.GetAllAsync().Returns(new List<User>());
            await _userAppService.RegisterAsync(registerUserViewModel);

            await _userRepository
                .Received(1)
                .AddAsync(Arg.Is<User>(u =>
                    u.Email == registerUserViewModel.Email && u.Password == registerUserViewModel.Password));
        }

        [Fact]
        public async Task GivenLoginAsync_WhenUserIsNotRegistered_ThenThrowException()
        {
            var loginUserViewModel = new LoginUserViewModel()
            {
                Email = "test@test.com",
                Password = "test123"
            };

            _userRepository.FindByEmailAndPasswordAsync(loginUserViewModel.Email, loginUserViewModel.Password).ReturnsNull();

            var exception = await Assert.ThrowsAsync<Exception>(() => _userAppService.LoginAsync(loginUserViewModel));
            exception.Message.Should().Be("Invalid email or password.");
        }

        [Fact]
        public async Task GivenLoginAsync_WhenThereIsNoErrors_ThenReturnsAccessToken()
        {
            var loginUserViewModel = new LoginUserViewModel()
            {
                Email = "test@test.com",
                Password = "test123"
            };

            var user = new User()
            {
                Email = loginUserViewModel.Email,
                Password = loginUserViewModel.Password
            };

            _userRepository.FindByEmailAndPasswordAsync(loginUserViewModel.Email, loginUserViewModel.Password).Returns(user);

            var response = await _userAppService.LoginAsync(loginUserViewModel);
            response.AccessToken.Should().NotBeNullOrEmpty();
        }
    }
}
