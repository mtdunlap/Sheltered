using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Api.Common;
using Client.Animals;
using Data;
using Data.Animals;

namespace Api.Animals;

/// <summary>
/// Represents a collection of endpoints for reading images.
/// </summary>
/// <param name="shelteredRepository">An <see cref="IShelteredRepository"/> to access the sheltered database.</param>
/// <param name="animalImageMapper">
/// An <see cref="IAnimalImageMapper"/> to map between <see cref="AnimalImageModel"/>s and
/// <see cref="AnimalImageEntity"/>s.
/// </param>
/// <param name="imageStore">An <see cref="IImageStore"/> for saving, reading, and deleting images.</param>
[ApiController]
[Route("[controller]")]
public sealed class ImageController(IShelteredRepository shelteredRepository, IAnimalImageMapper animalImageMapper,
    IImageStore imageStore) : ControllerBase
{
    /// <summary>
    /// Gets an image by id.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the image.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="StatusCodes.Status200OK"/> with a body containing an image if found;
    /// otherwise a <see cref="StatusCodes.Status404NotFound"/>.
    /// </returns>
    [HttpGet("{id:Guid}")]
    [EndpointName("GetImageById")]
    [EndpointSummary("Gets an image by id.")]
    [EndpointDescription("""
        Checks for an image with the provided id and returns a 200 with the body representing the image if found;
        otherwise returns a 404.
    """)]
    [ProducesResponseType<FileStreamResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute, Description("The id of the image.")] Guid id,
        CancellationToken cancellationToken = default)
    {
        var animalImageEntity = await shelteredRepository.GetAnimalImageByIdAsync(id, cancellationToken);
        if (animalImageEntity == AnimalImageEntity.NotFound)
        {
            return NotFound();
        }
        var file = imageStore.Get(animalImageEntity.Location);
        return File(file, animalImageEntity.ContentType);
    }

    /// <summary>
    /// Adds an image for the animal with the provided id.
    /// </summary>
    /// <param name="animalId">A <see cref="Guid"/> representing the id of the animal.</param>
    /// <param name="image">An <see cref="IFormFile"/> representing the image of the animal.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="StatusCodes.Status204NoContent"/> if the animal is updated successfully;
    /// otherwise a <see cref="StatusCodes.Status404NotFound"/> if no animal exists with the provided id.
    /// </returns>
    [HttpPost("{animalId:Guid}")]
    [EndpointName("AddImageForAnimalById")]
    [EndpointSummary("Adds an image for the animal with the provided id.")]
    [EndpointDescription("""
        Adds the provided image for the animal with the provided id using the provided animal model and returns a 204
        if the the animal is updated successfully;
        otherwise returns a 404 if no animal with the provided id exists.
    """)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post([FromRoute, Description("The id of the animal.")] Guid animalId,
        [FromForm, Description("The image to add.")] IFormFile image, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredRepository.GetAnimalByIdAsync(animalId, cancellationToken);
        if (animalEntity is null)
        {
            return NotFound();
        }

        var storedImageInfo = await imageStore.SaveAsync(image, cancellationToken);

        try
        {
            var created = animalImageMapper.Create(animalEntity, storedImageInfo);
            await shelteredRepository.AddAnimalImageAsync(created, cancellationToken);
            await shelteredRepository.SaveChangesAsync(cancellationToken);
            var mapped = animalImageMapper.Map(created, HttpContext);
            return CreatedAtAction(nameof(Get), new { id = mapped.Id.ToString() }, mapped);
        }
        catch
        {
            imageStore.Delete(storedImageInfo.Location);
            return Problem(string.Empty, string.Empty, (int)HttpStatusCode.InternalServerError);
        }
    }
}
