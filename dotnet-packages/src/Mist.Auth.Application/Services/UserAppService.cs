using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.ViewModels;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Infra.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserAppService(IMediatorHandler mediatorHandler, 
                              IOptions<AppSettings> appSettings,
                              SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager)
        {
            _mediatorHandler = mediatorHandler;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> LoginAsync(LoginUserViewModel loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (!result.Succeeded)
            {
                await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification("Login", "Usuário ou senha incorretos."));
                return false;
            }

            return true;
        }

        public async Task<bool> RegistrarAsync(RegisterUserViewModel registerUser)
        {
            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    await _mediatorHandler.RaiseDomainNotificationAsync(new DomainNotification("Registro", error.Description));
                }
                return false;
            }

            await _signInManager.SignInAsync(user, false);
            return true;
        }

        public async Task<LoginResponseViewModel> ObterJwtAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var claims = await _userManager.GetClaimsAsync(user);
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64));

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

            var encodedToken = tokenHandler.WriteToken(token);

            return new LoginResponseViewModel
            {
                AccessToken = encodedToken
            };
        }
    }
}
