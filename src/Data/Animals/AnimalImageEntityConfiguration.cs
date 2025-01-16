using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Common;

namespace Data.Animals;

/// <summary>
/// Entity Framework Core Configuration for <see cref="AnimalImageEntity"/>.
/// </summary>
public sealed class AnimalImageEntityConfiguration : EntityConfiguration<AnimalImageEntity>,
    IEntityTypeConfiguration<AnimalImageEntity>
{
    private const string TableName = "animal_images";

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        builder.ToTable(TableName, schema: ShelteredContext.Schema);

        ConfigureId(builder);
        ConfigureLocation(builder);
    }

    private static void ConfigureLocation(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        const string ColumnName = "location";
        const string ColumnType = "TEXT";

        builder
            .Property(animalImageEntity => animalImageEntity.Location)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }
}
