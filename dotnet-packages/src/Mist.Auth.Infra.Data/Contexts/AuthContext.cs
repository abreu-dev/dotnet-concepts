using Microsoft.EntityFrameworkCore;
using Mist.Auth.Domain.Entities;

namespace Mist.Auth.Infra.Data.Contexts
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
    }
}
