using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Common;

namespace Data.Animals;

/// <summary>
/// Entity Framework Core Configuration for <see cref="AnimalEntity"/>.
/// </summary>
public sealed class AnimalEntityConfiguration : EntityConfiguration<AnimalEntity>, IEntityTypeConfiguration<AnimalEntity>
{
    private const string TableName = "animals";

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AnimalEntity> builder)
    {
        builder.ToTable(TableName, schema: ShelteredContext.Schema);

        ConfigureId(builder);
        ConfigureName(builder);
        ConfigureKind(builder);
        ConfigureSex(builder);
    }

    private static void ConfigureName(EntityTypeBuilder<AnimalEntity> builder)
    {
        const string ColumnName = "name";
        const string ColumnType = "TEXT";
        const int MaxLength = 50;

        builder
            .Property(animalEntity => animalEntity.Name)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .HasMaxLength(MaxLength)
            .IsRequired(false);
    }

    private static void ConfigureKind(EntityTypeBuilder<AnimalEntity> builder)
    {
        const string ColumnName = "kind";
        const string ColumnType = "TEXT";

        builder
            .Property(animalEntity => animalEntity.Kind)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }

    private static void ConfigureSex(EntityTypeBuilder<AnimalEntity> builder)
    {
        const string ColumnName = "sex";
        const string ColumnType = "TEXT";

        builder
            .Property(animalEntity => animalEntity.Sex)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }
}
