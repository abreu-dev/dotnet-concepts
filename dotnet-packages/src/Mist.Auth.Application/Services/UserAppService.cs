using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Commands;
using Mist.Auth.Domain.Commands.UserCommands;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserAppService(IMediatorHandler mediatorHandler,
                              IUserRepository userRepository,
                              IMapper mapper,
                              IOptions<AppSettings> appSettings)
        {
            _mediatorHandler = mediatorHandler;
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public async Task<JwtResponseViewModel> LoginAsync(LoginUserViewModel loginUser)
        {
            var user = await _userRepository.FindByEmailAndPasswordAsync(loginUser.Email, loginUser.Password);

            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            return GetJwtData(user);
        }

        public async Task RegisterAsync(RegisterUserViewModel registerUser)
        {
            var command = new RegisterUserCommand()
            {
                Entity = _mapper.Map<User>(registerUser)
            };

            if (!command.IsValid())
            {
                await RaiseValidationErrorsAsync(command);
                return;
            }

            var users = await _userRepository.GetAllAsync();

            if (users.Any(u => u.Email == command.Entity.Email))
            {
                await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification(command.MessageType, "Email is already in use."));
                return;
            }

            await _userRepository.AddAsync(command.Entity);
        }

        private JwtResponseViewModel GetJwtData(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encondedToken = tokenHandler.WriteToken(token);

            return new JwtResponseViewModel
            {
                AccessToken = encondedToken
            };
        }

        private async Task RaiseValidationErrorsAsync(Command command)
        {
            foreach (var error in command.ValidationResult.Errors)
            {
                await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification(command.MessageType,
                    error.ErrorMessage));
            }
        }
    }
}
