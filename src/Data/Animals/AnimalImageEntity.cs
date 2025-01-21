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
        Location = string.Empty,
        ContentType = string.Empty,
        Height = 0,
        Width = 0,
        FileSize = 0
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

    /// <summary>
    /// Gets or inits the content type of the image.
    /// </summary>
    /// <value>The content type of the image.</value>
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets or inits the height of the image in pixels.
    /// </summary>
    /// <value>The height of the image in pixels.</value>
    public required uint Height { get; init; }

    /// <summary>
    /// Gets or inits the width of the image in pixels.
    /// </summary>
    /// <value>The width of the image in pixels.</value>
    public required uint Width { get; init; }

    /// <summary>
    /// Gets or inits the file size of the image in bytes.
    /// </summary>
    /// <value>The file size of the image in bytes.</value>
    public required ulong FileSize { get; init; }
}
