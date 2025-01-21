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
        ConfigureContentType(builder);
        ConfigureHeight(builder);
        ConfigureWidth(builder);
        ConfigureFileSize(builder);
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

    private static void ConfigureContentType(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        const string ColumnName = "content_type";
        const string ColumnType = "TEXT";

        builder
            .Property(animalImageEntity => animalImageEntity.ContentType)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }

    private static void ConfigureHeight(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        const string ColumnName = "height";
        const string ColumnType = "UNSIGNED INTEGER";

        builder
            .Property(animalImageEntity => animalImageEntity.Height)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }

    private static void ConfigureWidth(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        const string ColumnName = "width";
        const string ColumnType = "UNSIGNED INTEGER";

        builder
            .Property(animalImageEntity => animalImageEntity.Width)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }

    private static void ConfigureFileSize(EntityTypeBuilder<AnimalImageEntity> builder)
    {
        const string ColumnName = "file_size";
        const string ColumnType = "UNSIGNED BIG INTEGER";

        builder
            .Property(animalImageEntity => animalImageEntity.FileSize)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .IsRequired(true);
    }
}
