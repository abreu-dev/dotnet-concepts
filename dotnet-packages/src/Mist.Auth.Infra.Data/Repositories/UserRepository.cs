using Microsoft.EntityFrameworkCore;
using Mist.Auth.Domain.Entities;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mist.Auth.Infra.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            return await DbSet.AsNoTracking().AnyAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
