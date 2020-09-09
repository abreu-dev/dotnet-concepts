using Mist.Auth.Application.ViewModels;
using System;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Interfaces
{
    public interface IUserAppService : IDisposable
    {
        Task<LoginResponseViewModel> LoginAsync(LoginUserViewModel loginUser);
        Task RegisterAsync(RegisterUserViewModel registerUser);
    }
}
