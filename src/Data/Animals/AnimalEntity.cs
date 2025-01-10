using System;
using Microsoft.EntityFrameworkCore;
using Core.Animals;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
[EntityTypeConfiguration(typeof(AnimalEntityConfiguration))]
public sealed record class AnimalEntity : Entity, IEntity<AnimalEntity>
{
    public static AnimalEntity NotFound { get; } = new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Kind = AnimalKind.Unspecified
    };

    /// <summary>
    /// Gets or sets the name of the animal. May be null.
    /// </summary>
    /// <value>The name of the animal.</value>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    public required AnimalKind Kind { get; set; }
}
