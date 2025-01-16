using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Core.Animals;
using Data.Common;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
[EntityTypeConfiguration(typeof(AnimalEntityConfiguration))]
public sealed record class AnimalEntity : Entity
{
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

    /// <summary>
    /// Gets or sets a <see cref="List{AnimalImageEntity}"/> of <see cref="AnimalImageEntity"/>s.
    /// </summary>
    /// <value>The list of images of the animal.</value>
    public List<AnimalImageEntity> Images { get; set; } = [];
}
