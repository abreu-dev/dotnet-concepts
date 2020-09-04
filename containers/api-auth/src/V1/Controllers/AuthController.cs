using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mist.Auth.Application.ViewModels;
using System.Threading.Tasks;

namespace Auth.Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IConfiguration configuration,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    success = true,
                    data = loginUser
                });
            }

            return BadRequest(new 
            { 
                success = false,
                data = loginUser
            });
        }

    [HttpPost("registrar")]
    public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
    {
        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
                return Ok(new
                {
                    success = true,
                    data = registerUser
                });
            }

            return BadRequest(new
            {
                success = false,
                data = result.Errors
            });
        }
    }
}
