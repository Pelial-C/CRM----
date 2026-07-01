using CRM.Domain;
using CRM.Domain.Events;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : AggregateRoot<TKey>
{
    private readonly CrmDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public EfRepository(CrmDbContext dbContext, IDomainEventDispatcher eventDispatcher)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
        _eventDispatcher = eventDispatcher;
    }

    public async Task<TEntity> GetByIdAsync(TKey id)
    {
        var entity = await Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id));
        return entity!;
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    public Task<List<TEntity>> GetListAsync()
    {
        return Query().ToListAsync();
    }

    public async Task InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        await DispatchEventsAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
        await DispatchEventsAsync(entity);
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 保存成功后，收集并分发聚合根上积累的领域事件，然后清空事件列表。
    /// 事件分发失败不影响主事务（日志是非关键的附加行为）。
    /// </summary>
    private async Task DispatchEventsAsync(TEntity entity)
    {
        if (entity.DomainEvents.Count == 0) return;

        var events = entity.DomainEvents.ToList();
        entity.ClearDomainEvents();

        try
        {
            await _eventDispatcher.DispatchAsync(events);
        }
        catch
        {
            // 事件分发失败不抛出异常，避免影响主业务流程
        }
    }
}
