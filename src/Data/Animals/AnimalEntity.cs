using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Animals;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
[Table("animals", Schema = "sheltered")]
public sealed record class AnimalEntity : Entity, IEntity<AnimalEntity>
{
    /// <inheritdoc/>
    public static AnimalEntity None { get; } = new AnimalEntity { Name = null, Kind = AnimalKind.Unspecified };

    /// <summary>
    /// Gets or sets the name of the animal. May be null.
    /// </summary>
    /// <value>The name of the animal.</value>
    [Column("name", TypeName = "TEXT"), Required, MaxLength(50)]
    public required string? Name { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    [Column("kind", TypeName = "TEXT"), Required]
    public required AnimalKind Kind { get; set; }
}
