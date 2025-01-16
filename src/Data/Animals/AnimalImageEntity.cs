using System;
using Microsoft.EntityFrameworkCore;
using Data.Common;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an image of an animal.
/// </summary>
[EntityTypeConfiguration(typeof(AnimalImageEntityConfiguration))]
public sealed record class AnimalImageEntity : Entity
{
    /// <summary>
    /// Gets or inits the id of the <see cref="AnimalEntity"/> to which this image belongs.
    /// </summary>
    /// <value>The id of the <see cref="AnimalEntity"/> to which this image belongs.</value>
    public required Guid AnimalId { get; init; }

    /// <summary>
    /// Gets or inits the <see cref="AnimalEntity"/> of which this image is for.
    /// </summary>
    /// <value>The animal in the image.</value>
    public required AnimalEntity Animal { get; init; }

    /// <summary>
    /// Gets or inits a <see cref="Uri"/> representing the location of the image.
    /// </summary>
    /// <value>The location of the image.</value>
    public required Uri Location { get; init; }
}
