using System;
using System.Linq;
using Client.Animals;
using Data.Animals;

namespace Api.Animals;

/// <summary>
/// Represents a service for mapping to and from <see cref="AnimalModel"/>s and <see cref="AnimalEntity"/>s.
/// </summary>
public interface IAnimalMapper
{
    /// <summary>
    /// Creates a new <see cref="AnimalEntity"/> from an <see cref="AnimalModel"/>.
    /// </summary>
    /// <param name="animalModel">The <see cref="AnimalModel"/>.</param>
    /// <returns>A new <see cref="AnimalEntity"/>.</returns>
    AnimalEntity Create(AnimalModel animalModel);

    /// <summary>
    /// Creates a new <see cref="AnimalModel"/> from an <see cref="AnimalEntity"/>.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/>.</param>
    /// <returns>A new <see cref="AnimalModel"/>.</returns>
    AnimalModel Map(AnimalEntity animalEntity);

    /// <summary>
    /// Updates the <see cref="AnimalEntity"/> using the provided <see cref="AnimalModel"/>.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/>.</param>
    /// <param name="animalModel">The <see cref="AnimalModel"/>.</param>
    void Update(AnimalEntity animalEntity, AnimalModel animalModel);

    /// <summary>
    /// Adds an image to the <see cref="AnimalEntity"/>.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/> to which the image will be added.</param>
    /// <param name="location">A <see cref="Uri"/> representing the location of the image.</param>
    void AddImage(AnimalEntity animalEntity, Uri location);
}

/// <inheritdoc cref="IAnimalMapper"/>
public sealed class AnimalMapper(IAnimalImageMapper animalImageMapper) : IAnimalMapper
{
    /// <inheritdoc cref="IAnimalMapper.Create(AnimalModel)"/>
    public AnimalEntity Create(AnimalModel animalModel) => new()
    {
        Name = animalModel.Name,
        Kind = animalModel.Kind,
        Sex = animalModel.Sex
    };

    /// <inheritdoc cref="IAnimalMapper.Map(AnimalEntity)"/>
    public AnimalModel Map(AnimalEntity animalEntity) => new()
    {
        Name = animalEntity.Name,
        Kind = animalEntity.Kind,
        Sex = animalEntity.Sex,
        Images = [.. animalEntity.Images.Select(animalImageMapper.Map)]
    };

    /// <inheritdoc cref="IAnimalMapper.Update(AnimalEntity, AnimalModel)"/>
    public void Update(AnimalEntity animalEntity, AnimalModel animalModel)
    {
        animalEntity.Name = animalModel.Name;
        animalEntity.Kind = animalModel.Kind;
        animalEntity.Sex = animalModel.Sex;
    }

    /// <inheritdoc cref="IAnimalMapper.AddImage(AnimalEntity, Uri)"/>
    public void AddImage(AnimalEntity animalEntity, Uri location)
    {
        var animalImageEntity = animalImageMapper.Create(animalEntity.Id, location);
        animalEntity.Images.Add(animalImageEntity);
    }
}
