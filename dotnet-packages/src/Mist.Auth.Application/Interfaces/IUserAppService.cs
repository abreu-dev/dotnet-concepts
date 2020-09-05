using Mist.Auth.Application.ViewModels;
using System.Threading.Tasks;

namespace Mist.Auth.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<LoginResponseViewModel> LoginAsync(LoginUserViewModel loginUser);
        Task RegisterAsync(RegisterUserViewModel registerUser);
    }
}
