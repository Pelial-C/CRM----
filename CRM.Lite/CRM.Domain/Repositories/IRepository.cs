using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot<TKey>
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<List<TEntity>> GetListAsync();
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
