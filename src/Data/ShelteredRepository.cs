using System;
using System.Collections.Generic;
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
    /// Asynchronously adds an <see cref="AnimalImageEntity"/> to the repository.
    /// </summary>
    /// <param name="animalImageEntity">The <see cref="AnimalImageEntity"/> to add.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task AddAnimalImageAsync(AnimalImageEntity animalImageEntity, CancellationToken cancellationToken = default);

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
    /// Asynchronously retrieves an <see cref="AnimalImageEntity"/> with the provided id.
    /// </summary>
    /// <param name="id">The id of the <see cref="AnimalImageEntity"/> to find.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation and contains the <see cref="AnimalImageEntity"/>
    /// if found; otherwise contains <see cref="AnimalImageEntity.NotFound"/>.
    /// </returns>
    Task<AnimalImageEntity> GetAnimalImageByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all <see cref="AnimalEntity"/>s.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation and contains the <see cref="List{T}"/> of
    /// <see cref="AnimalEntity"/> if found; otherwise contains null.
    /// </returns>
    Task<List<AnimalEntity>> ListAnimalsAsync(CancellationToken cancellationToken = default);

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

    /// <inheritdoc cref="IShelteredRepository.AddAnimalImageAsync(AnimalImageEntity, CancellationToken)"/>
    public async Task AddAnimalImageAsync(AnimalImageEntity animalImageEntity,
        CancellationToken cancellationToken = default)
    {
        _ = await shelteredContext.AnimalImages.AddAsync(animalImageEntity, cancellationToken);
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
        return await shelteredContext.Animals.Include(animalEntity => animalEntity.Images).SingleOrDefaultAsync(animalEntity => animalEntity.Id == id, cancellationToken);
    }

    /// <inheritdoc cref="IShelteredRepository.GetAnimalImageByIdAsync(Guid, CancellationToken)"/>
    public async Task<AnimalImageEntity> GetAnimalImageByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await shelteredContext.AnimalImages.SingleOrDefaultAsync(animalImageEntity => animalImageEntity.Id == id,
            cancellationToken) ?? AnimalImageEntity.NotFound;
    }

    /// <inheritdoc cref="IShelteredRepository.ListAnimalsAsync(CancellationToken)"/>
    public async Task<List<AnimalEntity>> ListAnimalsAsync(CancellationToken cancellationToken = default)
    {
        return await shelteredContext.Animals.Include(animalEntity => animalEntity.Images).ToListAsync(cancellationToken);
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
