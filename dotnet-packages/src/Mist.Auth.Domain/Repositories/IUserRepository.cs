using Mist.Auth.Domain.Entities;
using System.Threading.Tasks;

namespace Mist.Auth.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByEmailAndPasswordAsync(string email, string password);
        Task<User> FindByEmailAsync(string email);
    }
}
