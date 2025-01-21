using System.Linq;
using Client.Animals;
using Data.Animals;
using Microsoft.AspNetCore.Http;

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
    /// <param name="httpContext">The <see cref="HttpContext"/> of the request.</param>
    /// <returns>A new <see cref="AnimalModel"/>.</returns>
    AnimalModel Map(AnimalEntity animalEntity, HttpContext httpContext);

    /// <summary>
    /// Updates the <see cref="AnimalEntity"/> using the provided <see cref="AnimalModel"/>.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/>.</param>
    /// <param name="animalModel">The <see cref="AnimalModel"/>.</param>
    void Update(AnimalEntity animalEntity, AnimalModel animalModel);
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

    /// <inheritdoc cref="IAnimalMapper.Map(AnimalEntity, HttpContext)"/>
    public AnimalModel Map(AnimalEntity animalEntity, HttpContext httpContext) => new()
    {
        Name = animalEntity.Name,
        Kind = animalEntity.Kind,
        Sex = animalEntity.Sex,
        Images = [.. animalEntity.Images
            .Select(animalImageEntity => animalImageMapper.Map(animalImageEntity, httpContext))]
    };

    /// <inheritdoc cref="IAnimalMapper.Update(AnimalEntity, AnimalModel)"/>
    public void Update(AnimalEntity animalEntity, AnimalModel animalModel)
    {
        animalEntity.Name = animalModel.Name;
        animalEntity.Kind = animalModel.Kind;
        animalEntity.Sex = animalModel.Sex;
    }
}
