using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Data;

namespace Api.UnitTests;

internal abstract class EntityEqualityComparer<TEntity>(bool includeId, bool includeCreated, bool includeLastUpdated) : IEqualityComparer<TEntity> where TEntity : Entity
{
    public virtual bool Equals(TEntity? x, TEntity? y)
    {
        if (x is not null && y is not null)
        {
            var idEquals = includeId && x.Id == y.Id;
            var createdEquals = includeCreated && x.Created == y.Created;
            var lastUpdatedEquals = includeLastUpdated && x.LastUpdated == y.LastUpdated;
            return idEquals && createdEquals && lastUpdatedEquals;
        }

        return x is null && y is null;
    }

    public abstract int GetHashCode([DisallowNull] TEntity obj);
}
