using System;
using Client.Animals;
using Data.Animals;

namespace Api.Animals;

/// <summary>
/// Represents a service for mapping to and from <see cref="AnimalImageModel"/>s and <see cref="AnimalImageEntity"/>s.
/// </summary>
public interface IAnimalImageMapper
{
    /// <summary>
    /// Creates a new <see cref="AnimalImageEntity"/> from an <paramref name="animalId"/> and <paramref name="location"/>.
    /// </summary>
    /// <param name="animalId">
    /// A <see cref="Guid"/> representing the id of the <see cref="AnimalEntity"/> to which the image belongs.
    /// </param>
    /// <param name="location">A <see cref="Uri"/> representing the location of the image.</param>
    /// <returns>A new <see cref="AnimalImageEntity"/>.</returns>
    AnimalImageEntity Create(Guid animalId, Uri location);

    /// <summary>
    /// Creates a new <see cref="AnimalModel"/> from an <see cref="AnimalEntity"/>.
    /// </summary>
    /// <param name="animalImageEntity">The <see cref="AnimalImageEntity"/>.</param>
    /// <returns>A new <see cref="AnimalImageModel"/>.</returns>
    AnimalImageModel Map(AnimalImageEntity animalImageEntity);
}

/// <inheritdoc cref="IAnimalImageMapper"/>
public sealed class AnimalImageMapper : IAnimalImageMapper
{
    /// <inheritdoc cref="IAnimalImageMapper.Create(Guid, Uri)"/>
    public AnimalImageEntity Create(Guid animalId, Uri location) => new()
    {
        AnimalId = animalId,
        Location = location.ToString()
    };

    /// <inheritdoc cref="IAnimalImageMapper.Map(AnimalImageEntity)"/>
    public AnimalImageModel Map(AnimalImageEntity animalImageEntity) => new()
    {
        Location = animalImageEntity.Location
    };
}
