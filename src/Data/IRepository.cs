using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Provides a mechanism for adding, retrieving, and deleting <see cref="Entity"/>s to and from a <typeparamref name="TContext"/>.
/// </summary>
/// <typeparam name="TContext">The typed <see cref="DbContext"/>.</typeparam>
public interface IRepository<TContext> where TContext : DbContext
{
    /// <summary>
    /// Asynchronously adds the supplied <typeparamref name="TEntity"/> to the <typeparamref name="TContext"/>,
    /// optionally allowing cancellation via the supplied <see cref="CancellationToken"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="Entity"/> being added.</typeparam>
    /// <param name="entity">The <typeparamref name="TEntity"/> to add to the <see cref="IRepository{TContext}"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    ValueTask<bool> ExistsByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    ValueTask<TEntity> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    void Remove<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>;

    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity, IEntity<TEntity>;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    void Update<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>;
}
