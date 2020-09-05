using Mist.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mist.Auth.Domain.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task Adicionar(TEntity entity);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task<int> SaveChanges();
    }
}
