using System;
using Data;

namespace Api;

/// <summary>
/// A base mapper for updating the <see cref="Entity.LastUpdated"/> <see cref="DateTime"/> on an <see cref="Entity"/>.
/// </summary>
public abstract class EntityMapper
{
    /// <summary>
    /// Updates the <see cref="Entity.LastUpdated"/> <see cref="DateTime"/> on the provided <see cref="Entity"/>.
    /// </summary>
    /// <param name="entity">The <see cref="Entity"/> to update.</param>
    public void Update(Entity entity)
    {
        entity.LastUpdated = DateTime.UtcNow;
    }
}
