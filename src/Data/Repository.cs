using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

public sealed class Repository<TContext>(TContext dbContext) : IRepository<TContext> where TContext : DbContext
{
    public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>
    {
        _ = await dbContext.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>
    {
        await dbContext.AddRangeAsync(entities, cancellationToken);
    }

    public async ValueTask<bool> ExistsByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>
    {
        return await GetByIdAsync<TEntity>(id, cancellationToken) != TEntity.None;
    }

    public async ValueTask<TEntity> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>
    {
        return await dbContext.FindAsync<TEntity>([id], cancellationToken) ?? TEntity.None;
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>
    {
        _ = dbContext.Remove(entity);
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity, IEntity<TEntity>
    {
        dbContext.RemoveRange(entities);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _ = await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>
    {
        _ = dbContext.Update(entity);
    }
}
