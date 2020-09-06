using Mist.Auth.Application.ViewModels;
using System;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Interfaces
{
    public interface IUserAppService : IDisposable
    {
        Task<JwtResponseViewModel> LoginAsync(LoginUserViewModel loginUser);
        Task RegisterAsync(RegisterUserViewModel registerUser);
    }
}
