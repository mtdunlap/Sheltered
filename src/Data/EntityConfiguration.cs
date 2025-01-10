using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Data;

public abstract class EntityConfiguration<TEntity> where TEntity : Entity, IEntity<TEntity>
{
    public abstract string Table { get; }

    public const string IdColumnName = "id";

    public const string IdColumnType = "uuid";

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        _ = builder
            .ToTable(Table);

        _ = builder
            .HasKey(entity => entity.Id);

        _ = builder
            .Property(entity => entity.Id)
                .HasColumnName(IdColumnName)
                .HasColumnType(IdColumnType)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
    }
}
