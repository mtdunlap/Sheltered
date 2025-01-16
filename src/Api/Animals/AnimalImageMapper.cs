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
    /// Creates a new <see cref="AnimalImageEntity"/> from an <see cref="AnimalImageModel"/>.
    /// </summary>
    /// <param name="animalId">
    /// A <see cref="Guid"/> representing the id of the <see cref="AnimalEntity"/> to which the image belongs.
    /// </param>
    /// <param name="location">The location of the image.</param>
    /// <returns>A new <see cref="AnimalImageEntity"/>.</returns>
    AnimalImageEntity Create(Guid animalId, string location);

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
    /// <inheritdoc cref="IAnimalImageMapper.Create(Guid, string)"/>
    public AnimalImageEntity Create(Guid animalId, string location) => new()
    {
        AnimalId = animalId,
        Location = location
    };

    /// <inheritdoc cref="IAnimalImageMapper.Map(AnimalImageEntity)"/>
    public AnimalImageModel Map(AnimalImageEntity animalImageEntity) => new()
    {
        Location = animalImageEntity.Location
    };
}
