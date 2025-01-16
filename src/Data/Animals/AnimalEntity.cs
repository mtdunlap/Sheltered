using System;
using System.Diagnostics.CodeAnalysis;
using Core.Animals;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
public sealed record class AnimalEntity
{
    /// <summary>
    /// Gets or inits the id of the animal.
    /// </summary>
    /// <remarks>
    /// The Id is not intended to be set/init by users, but is necessary so that EF Core can set the Id.
    /// </remarks>
    /// <value>The id of the animal.</value>
    public Guid Id
    {
        get;

        [ExcludeFromCodeCoverage(Justification = "Cannot test a private initter.")]
        private init;
    } = Guid.Empty;

    /// <summary>
    /// Gets or sets the name of the animal. May be null.
    /// </summary>
    /// <value>The name of the animal.</value>
    public required string? Name { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    public required AnimalKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AnimalSex"/> of the animal.
    /// </summary>
    /// <value>The sex of the animal.</value>
    public required AnimalSex Sex { get; set; }
}
