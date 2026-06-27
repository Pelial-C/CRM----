using System.Linq.Expressions;

namespace CRM.Domain.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : AggregateRoot<TKey>
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<List<TEntity>> GetListAsync();
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
}
