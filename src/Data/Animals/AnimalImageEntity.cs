using System;
using Microsoft.EntityFrameworkCore;
using Data.Common;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an image of an animal.
/// </summary>
[EntityTypeConfiguration(typeof(AnimalImageEntityConfiguration))]
public sealed record class AnimalImageEntity : Entity, IEntity<AnimalImageEntity>
{
    /// <inheritdoc/>
    public static AnimalImageEntity NotFound { get; } = new()
    {
        AnimalId = Guid.Empty,
        Animal = AnimalEntity.NotFound,
        Location = string.Empty
    };

    /// <summary>
    /// Gets or inits the id of the <see cref="AnimalEntity"/> to which this image belongs.
    /// </summary>
    /// <value>The id of the <see cref="AnimalEntity"/> to which this image belongs.</value>
    public Guid AnimalId { get; init; } = Guid.Empty;

    /// <summary>
    /// Gets or inits the <see cref="AnimalEntity"/> of which this image is for.
    /// </summary>
    /// <value>The animal in the image.</value>
    public AnimalEntity Animal { get; init; } = AnimalEntity.NotFound;

    /// <summary>
    /// Gets or inits the location of the image.
    /// </summary>
    /// <value>The location of the image.</value>
    public required string Location { get; init; }
}
