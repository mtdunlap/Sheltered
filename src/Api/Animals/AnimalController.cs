using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Client.Animals;
using Data.Sheltered;
using Data.Sheltered.Animals;

namespace Api.Animals;

/// <summary>
/// Represents a collection of endpoints for creating, reading, updating, or deleting information about animals.
/// </summary>
/// <param name="shelteredContext">A <see cref="ShelteredContext"/> to access the sheltered database.</param>
/// <param name="animalMapper">An <see cref="IAnimalMapper"/> to map between <see cref="AnimalModel"/>s and <see cref="AnimalEntity"/>s.</param>
[ApiController]
[Route("[controller]")]
public sealed class AnimalController(ShelteredContext shelteredContext, IAnimalMapper animalMapper) : ControllerBase
{
    /// <summary>
    /// Determines if an animal with the provided id exists.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the animal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="StatusCodes.Status204NoContent"/> if found; otherwise a <see cref="StatusCodes.Status404NotFound"/>.</returns>
    [HttpHead("{id:Guid}")]
    [EndpointName("AnimalExistsById")]
    [EndpointSummary("Determines if an animal with the provided id exists.")]
    [EndpointDescription("Checks for the existence of an animal with the provided id and returns a 204 if found; otherwise returns a 404.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Head([FromRoute, Description("The id of the animal.")] Guid id, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredContext.FindAsync<AnimalEntity>(id, cancellationToken);
        if (animalEntity is null)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Gets an animal by id.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the animal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="StatusCodes.Status200OK"/> with a body containing an <see cref="AnimalModel"/> if found; otherwise a <see cref="StatusCodes.Status404NotFound"/>.</returns>
    [HttpGet("{id:Guid}")]
    [EndpointName("GetAnimalById")]
    [EndpointSummary("Gets an animal by id.")]
    [EndpointDescription("Checks for an animal with the provided id and returns a 200 with the body representing the animal if found; otherwise returns a 404.")]
    [ProducesResponseType<AnimalModel>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute, Description("The id of the animal.")] Guid id, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredContext.FindAsync<AnimalEntity>(id, cancellationToken);
        if (animalEntity is null)
        {
            return NotFound();
        }
        var animal = animalMapper.Map(animalEntity);
        return Ok(animal);
    }

    /// <summary>
    /// Creates a new animal.
    /// </summary>
    /// <param name="animalModel">An <see cref="AnimalModel"/> representing the animal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="StatusCodes.Status201Created"/> with a body representing the newly created animal and the location header set to the location of the animal.</returns>
    [HttpPost]
    [EndpointName("CreateAnimal")]
    [EndpointSummary("Creates a new animal.")]
    [EndpointDescription("Creates a new animal from the provided animal model and returns a 201 with a body representing the newly created animal.")]
    [ProducesResponseType<AnimalModel>(StatusCodes.Status201Created, "application/json")]
    public async Task<IActionResult> Post([FromBody, Description("The animal to create.")] AnimalModel animalModel, CancellationToken cancellationToken = default)
    {
        var animalEntity = animalMapper.Create(animalModel);
        _ = await shelteredContext.AddAsync(animalEntity, cancellationToken);
        _ = await shelteredContext.SaveChangesAsync(cancellationToken);
        var createdEntity = await shelteredContext.FindAsync<AnimalEntity>(animalEntity.Id, cancellationToken) ?? throw new InvalidOperationException();
        var createdAnimal = animalMapper.Map(createdEntity);
        return CreatedAtAction(nameof(Get), new { id = animalEntity.Id.ToString() }, createdAnimal);
    }

    /// <summary>
    /// Updates an existing animal by id and model.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the animal.</param>
    /// <param name="animalModel">An <see cref="AnimalModel"/> representing the updated animal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="StatusCodes.Status204NoContent"/> if the animal is updated successfully; otherwise a <see cref="StatusCodes.Status404NotFound"/> if no animal exists with the provided id.</returns>
    [HttpPut("{id:Guid}")]
    [EndpointName("UpdateAnimalById")]
    [EndpointSummary("Updates an existing animal by id and model.")]
    [EndpointDescription("Updates the animal with the provided id using the provided animal model and returns a 204 if the the animal is updated successfully; otherwise returns a 404 if no animal with the provided id exists.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromRoute, Description("The id of the animal.")] Guid id, [FromBody, Description("The updated animal.")] AnimalModel animalModel, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredContext.FindAsync<AnimalEntity>(id, cancellationToken);
        if (animalEntity is null)
        {
            return NotFound();
        }
        animalMapper.Update(animalEntity, animalModel);
        _ = shelteredContext.Update(animalEntity);
        _ = await shelteredContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes an animal by id.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the animal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="StatusCodes.Status204NoContent"/> if the animal is found and deleted; otherwise a <see cref="StatusCodes.Status404NotFound"/>.</returns>
    [HttpDelete("{id:Guid}")]
    [EndpointName("DeleteAnimalById")]
    [EndpointSummary("Deletes an animal by id.")]
    [EndpointDescription("Deletes the animal with the provided id and returns a 204 if found and deleted; otherwise returns a 404 if no animal is found with the provided id.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute, Description("The id of the animal.")] Guid id, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredContext.FindAsync<AnimalEntity>(id, cancellationToken);
        if (animalEntity is null)
        {
            return NotFound();
        }
        shelteredContext.Remove(animalEntity);
        _ = await shelteredContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
