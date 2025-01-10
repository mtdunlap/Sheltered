using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Animals;

public sealed class AnimalEntityConfiguration : EntityConfiguration<AnimalEntity>,
    IEntityTypeConfiguration<AnimalEntity>
{
    public override string Table { get; } = "animals";

    public const string NameColumnName = "name";

    public const string NameColumnType = "text";

    public const int NameMaxLength = 50;

    public const string KindColumnName = "kind";

    public override void Configure(EntityTypeBuilder<AnimalEntity> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(animalEntity => animalEntity.Name)
                .HasColumnName(NameColumnName)
                .HasColumnType(NameColumnType)
                .HasMaxLength(NameMaxLength)
                .IsRequired(false);

        _ = builder
            .Property(animalEntity => animalEntity.Kind)
                .HasColumnName(KindColumnName)
                .HasColumnType(ShelteredContext.AnimalKindEnumTypeName)
                .IsRequired();
    }
}
