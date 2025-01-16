using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Common;

/// <summary>
/// Entity Framework Core Configuration for <see cref="Entity"/>.
/// </summary>
/// <typeparam name="TEntity">The <see cref="Entity"/> type.</typeparam>
public abstract class EntityConfiguration<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Configures the Id of the <see cref="Entity"/>.
    /// </summary>
    /// <param name="builder">An <see cref="EntityTypeBuilder{Entity}"/> for configuring the <see cref="Entity"/>.</param>
    protected static void ConfigureId(EntityTypeBuilder<TEntity> builder)
    {
        const string ColumnName = "id";
        const string ColumnType = "TEXT";

        builder.HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.Id)
            .HasColumnName(ColumnName)
            .HasColumnType(ColumnType)
            .ValueGeneratedOnAdd()
            .IsRequired(true);
    }
}
