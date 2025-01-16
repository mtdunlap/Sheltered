using Microsoft.EntityFrameworkCore;
using Data.Animals;

namespace Data;

/// <summary>
/// Represents a session with the sheltered database.
/// </summary>
/// <param name="dbContextOptions"></param>
public sealed class ShelteredContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    /// <summary>
    /// The name of the schema the <see cref="ShelteredContext"/> represents.
    /// </summary>
    public const string Schema = "sheltered";

    /// <summary>
    /// Gets or inits the <see cref="DbSet{AnimalEntity}"/> of <see cref="AnimalEntity"/>.
    /// </summary>
    /// <value>Represents the animals table of the sheltered database.</value>
    public DbSet<AnimalEntity> Animals { get; init; } = null!;

    /// <summary>
    /// Gets or inits the <see cref="DbSet{AnimalImageEntity}"/> of <see cref="AnimalImageEntity"/>.
    /// </summary>
    /// <value>Represents the animal images table of the sheltered database.</value>
    public DbSet<AnimalImageEntity> AnimalImages { get; init; } = null!;
}
