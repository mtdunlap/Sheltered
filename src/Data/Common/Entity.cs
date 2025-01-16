using System;
using System.Diagnostics.CodeAnalysis;

namespace Data.Common;

/// <summary>
/// Represents a base class for all entities.
/// </summary>
public abstract record class Entity
{
    /// <summary>
    /// Gets or inits the id of the entity.
    /// </summary>
    /// <remarks>
    /// The Id is not intended to be set/init by users, but is necessary so that EF Core can set the Id.
    /// </remarks>
    /// <value>The id of the entity.</value>
    public Guid Id
    {
        get;

        [ExcludeFromCodeCoverage(Justification = "Cannot test a private initter.")]
        private init;
    } = Guid.Empty;
}
