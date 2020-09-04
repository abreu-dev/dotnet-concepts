using Mist.Auth.Application.ViewModels;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<bool> LoginAsync(LoginUserViewModel loginUser);
        Task<bool> RegistrarAsync(RegisterUserViewModel registerUser);
        Task<LoginResponseViewModel> ObterJwtAsync(string email);
    }
}
