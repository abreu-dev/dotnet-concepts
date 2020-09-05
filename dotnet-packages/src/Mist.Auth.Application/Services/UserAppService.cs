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
using System.Collections.Generic;
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
        private readonly AppSettings _appSettings;

        public UserAppService(IMediatorHandler mediatorHandler,
                              IUserRepository userRepository,
                              IOptions<AppSettings> appSettings)
        {
            _mediatorHandler = mediatorHandler;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public async Task<LoginResponseViewModel> LoginAsync(LoginUserViewModel loginUser)
        {
            var autenticado = await _userRepository.AuthenticateAsync(loginUser.Email, loginUser.Password);

            if (autenticado)
            {
                return new LoginResponseViewModel() 
                { 
                    Success = true,
                    Data = await ObterTokenAsync(loginUser.Email)
                };
            }

            return new LoginResponseViewModel()
            {
                Success = false,
                Errors = new List<string> { "Email ou senha inválidos." }
            };
        }

        public async Task RegistrarAsync(RegisterUserViewModel registerUser)
        {
            var command = new RegistrarUserCommand()
            {
                Entity = new User
                {
                    Email = registerUser.Email,
                    Password = registerUser.Password
                }
            };

            if (!command.IsValid())
            {
                await RaiseValidationErrorsAsync(command);
                return;
            }

            var listaUser = await _userRepository.ObterTodos();

            if (listaUser.Any(u => u.Email == command.Entity.Email))
            {
                await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification(command.MessageType, "Email já cadastrado."));
                return;
            }

            await _userRepository.Adicionar(command.Entity);
        }

        private async Task<JwtDataViewModel> ObterTokenAsync(string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);

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
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encondedToken = tokenHandler.WriteToken(token);

            return new JwtDataViewModel
            {
                AccessToken = encondedToken
            };
        }

        protected async Task RaiseValidationErrorsAsync(Command command)
        {
            foreach (var error in command.ValidationResult.Errors)
            {
                await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification(command.MessageType,
                    error.ErrorMessage));
            }
        }
    }
}
