using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data;

public interface IRepository<TContext> where TContext : DbContext
{
    Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    ValueTask<bool> ExistsByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    ValueTask<TEntity> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : Entity, IEntity<TEntity>;

    void Remove<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>;

    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity, IEntity<TEntity>;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    void Update<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>;
}
