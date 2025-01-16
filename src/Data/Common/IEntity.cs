using System;

namespace Data.Common;

/// <summary>
/// Represents an entity.
/// </summary>
/// <typeparam name="TSelf">The type of the entity.</typeparam>
public interface IEntity<TSelf> where TSelf : IEntity<TSelf>
{
    /// <summary>
    /// Gets an uninitialized value of <typeparamref name="TSelf"/> to return when the entity if not found.
    /// </summary>
    /// <value>An uninitialized <typeparamref name="TSelf"/>.</value>
    static abstract TSelf NotFound { get; }

    /// <summary>
    /// Gets or inits the id of the entity.
    /// </summary>
    /// <remarks>
    /// The Id is not intended to be set/init by users, but is necessary so that EF Core can set the Id.
    /// </remarks>
    /// <value>The id of the entity.</value>
    Guid Id { get; }
}
