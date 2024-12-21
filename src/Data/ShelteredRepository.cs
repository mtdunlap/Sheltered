using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Animals;

namespace Data;

/// <summary>
/// Represents a repository pattern for the <see cref="ShelteredContext"/>.
/// </summary>
public interface IShelteredRepository
{
    /// <summary>
    /// Asynchronously adds an <see cref="AnimalEntity"/> to the repository.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/> to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task AddAnimalAsync(AnimalEntity animalEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines if an <see cref="AnimalEntity"/> with the provided id exists in the repository.
    /// </summary>
    /// <param name="id">The id of the <see cref="AnimalEntity"/> to find.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation and contains true if found; otherwise false.</returns>
    Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves an <see cref="AnimalEntity"/> with the provided id. May return null.
    /// </summary>
    /// <param name="id">The id of the <see cref="AnimalEntity"/> to find.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation and contains the <see cref="AnimalEntity"/> if found; otherwise contains null.</returns>
    Task<AnimalEntity?> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the provided <see cref="AnimalEntity"/> from the repository.
    /// </summary>
    /// <param name="animalEntity">The <see cref="AnimalEntity"/> to remove.</param>
    void RemoveAnimal(AnimalEntity animalEntity);

    /// <summary>
    /// Asynchronously saves all changes to the repository.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the provided <see cref="AnimalEntity"/> in the repository.
    /// </summary>
    /// <param name="animalEntity">The updated <see cref="AnimalEntity"/>.</param>
    void UpdateAnimal(AnimalEntity animalEntity);
}

/// <inheritdoc cref="IShelteredRepository"/>
/// <param name="shelteredContext">The underlying <see cref="ShelteredContext"/> the repository uses for its data store.</param>
public sealed class ShelteredRepository(ShelteredContext shelteredContext) : IShelteredRepository
{
    /// <inheritdoc cref="IShelteredRepository.AddAnimalAsync(AnimalEntity, CancellationToken)"/>
    public async Task AddAnimalAsync(AnimalEntity animalEntity, CancellationToken cancellationToken = default)
    {
        _ = await shelteredContext.Animals.AddAsync(animalEntity, cancellationToken);
    }

    /// <inheritdoc cref="IShelteredRepository.AnimalExistsByIdAsync(Guid, CancellationToken)"/>
    public async Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var animalEntity = await shelteredContext.Animals.SingleOrDefaultAsync(animalEntity => animalEntity.Id == id, cancellationToken);
        return animalEntity is not null;
    }

    /// <inheritdoc cref="IShelteredRepository.GetAnimalByIdAsync(Guid, CancellationToken)"/>
    public async Task<AnimalEntity?> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await shelteredContext.Animals.SingleOrDefaultAsync(animalEntity => animalEntity.Id == id, cancellationToken);
    }

    /// <inheritdoc cref="IShelteredRepository.RemoveAnimal(AnimalEntity)"/>
    public void RemoveAnimal(AnimalEntity animalEntity)
    {
        _ = shelteredContext.Animals.Remove(animalEntity);
    }

    /// <inheritdoc cref="IShelteredRepository.SaveChangesAsync(CancellationToken)"/>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _ = await shelteredContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc cref="IShelteredRepository.UpdateAnimal(AnimalEntity)"/>
    public void UpdateAnimal(AnimalEntity animalEntity)
    {
        _ = shelteredContext.Update(animalEntity);
    }
}
