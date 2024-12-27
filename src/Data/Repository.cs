using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Represents a repository pattern for accessing a <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TContext">The typed <see cref="DbContext"/>.</typeparam>
/// <param name="dbContext">The <typeparamref name="TContext"/> for interacting with the underlying database.</param>
public sealed class Repository<TContext>(TContext dbContext) : IRepository<TContext> where TContext : DbContext
{
    /// <summary>
    /// Asynchronously adds the supplied <typeparamref name="TEntity"/> to the underlying <typeparamref name="TContext"/>,
    /// optionally allowing cancellation via the supplied <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="Entity"/> being added.</typeparam>
    /// <param name="entity">The <typeparamref name="TEntity"/> to add to the <see cref="DbContext"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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
