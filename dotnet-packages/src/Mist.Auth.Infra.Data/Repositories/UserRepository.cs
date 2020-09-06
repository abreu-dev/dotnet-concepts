using Microsoft.EntityFrameworkCore;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Data.Common;
using Mist.Auth.Infra.Data.Contexts;
using System.Threading.Tasks;

namespace Mist.Auth.Infra.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AuthContext context) : base(context) { }

        public async Task<User> FindByEmailAndPasswordAsync(string email, string password)
        {
            var user = await DbSet.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                if (SecurePasswordHasher.Verify(password, user.Password))
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await DbSet.AsNoTracking().SingleOrDefaultAsync();
        }

        public override async Task AddAsync(User user)
        {
            user.Password = SecurePasswordHasher.Hash(user.Password);

            DbSet.Add(user);
            await SaveChangesAsync();
        }
    }
}
