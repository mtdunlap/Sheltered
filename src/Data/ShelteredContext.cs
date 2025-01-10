using Microsoft.EntityFrameworkCore;
using Data.Animals;
using Core.Animals;

namespace Data;

/// <summary>
/// Represents a session with the sheltered database.
/// </summary>
/// <param name="dbContextOptions"></param>
public sealed class ShelteredContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public const string Schema = "sheltered";

    public const string AnimalKindEnumTypeName = "animal_kind";

    /// <summary>
    /// Gets or inits the <see cref="DbSet{AnimalEntity}"/> of <see cref="AnimalEntity"/>.
    /// </summary>
    /// <value>Represents the animals table of the sheltered database.</value>
    public DbSet<AnimalEntity> Animals { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.HasDefaultSchema(Schema);
        _ = modelBuilder.HasPostgresEnum<AnimalKind>(Schema, AnimalKindEnumTypeName);
    }
}
