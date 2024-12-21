using Microsoft.EntityFrameworkCore;
using Data.Sheltered.Animals;

namespace Data.Sheltered;

/// <summary>
/// Represents a session with the sheltered database.
/// </summary>
/// <param name="dbContextOptions"></param>
public sealed class ShelteredContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    /// <summary>
    /// Gets or inits the <see cref="DbSet{AnimalEntity}"/> of <see cref="AnimalEntity"/>.
    /// </summary>
    /// <value>Represents the animals table of the sheltered database.</value>
    public DbSet<AnimalEntity> Animals { get; init; } = null!;
}
