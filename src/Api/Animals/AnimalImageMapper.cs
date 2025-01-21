using Microsoft.AspNetCore.Http;
using Api.Common;
using Client.Animals;
using Data.Animals;

namespace Api.Animals;

/// <summary>
/// Represents a service for mapping to and from <see cref="AnimalImageModel"/>s and <see cref="AnimalImageEntity"/>s.
/// </summary>
public interface IAnimalImageMapper
{
    /// <summary>
    /// Creates a new <see cref="AnimalImageEntity"/> from an <paramref name="animalEntity"/> and
    /// <paramref name="storedImageInfo"/>.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/> to which the image belongs.</param>
    /// <param name="storedImageInfo">
    /// A <see cref="StoredImageInfo"/> representing information about the stored image.
    /// </param>
    /// <returns>A new <see cref="AnimalImageEntity"/>.</returns>
    AnimalImageEntity Create(AnimalEntity animalEntity, StoredImageInfo storedImageInfo);

    /// <summary>
    /// Creates a new <see cref="AnimalModel"/> from an <see cref="AnimalEntity"/>.
    /// </summary>
    /// <param name="animalImageEntity">The <see cref="AnimalImageEntity"/>.</param>
    /// <param name="httpContext">The <see cref="HttpContext"/> of the request.</param>
    /// <returns>A new <see cref="AnimalImageModel"/>.</returns>
    AnimalImageModel Map(AnimalImageEntity animalImageEntity, HttpContext httpContext);
}

/// <inheritdoc cref="IAnimalImageMapper"/>
public sealed class AnimalImageMapper(IImageStore imageStore) : IAnimalImageMapper
{
    /// <inheritdoc cref="IAnimalImageMapper.Create(AnimalEntity, StoredImageInfo)"/>
    public AnimalImageEntity Create(AnimalEntity animalEntity, StoredImageInfo storedImageInfo) => new()
    {
        AnimalId = animalEntity.Id,
        Animal = animalEntity,
        Location = storedImageInfo.Location.ToString(),
        ContentType = storedImageInfo.ContentType,
        Height = storedImageInfo.Height,
        Width = storedImageInfo.Width,
        FileSize = storedImageInfo.FileSize
    };

    /// <inheritdoc cref="IAnimalImageMapper.Map(AnimalImageEntity, HttpContext)"/>
    public AnimalImageModel Map(AnimalImageEntity animalImageEntity, HttpContext httpContext) => new()
    {
        Id = animalImageEntity.Id,
        Location = imageStore.GetLocation(httpContext, new { id = animalImageEntity.Id })
            ?? animalImageEntity.Location,
        ContentType = animalImageEntity.ContentType,
        Height = animalImageEntity.Height,
        Width = animalImageEntity.Width,
        FileSize = animalImageEntity.FileSize
    };
}
