using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Animals;

namespace Data.Sheltered.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
[Table("animals", Schema = "sheltered")]
public sealed record class AnimalEntity
{
    /// <summary>
    /// Gets or inits the id of the animal.
    /// </summary>
    /// <value>The id of the animal.</value>
    [Column("id", TypeName = "TEXT"), Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private init; } = Guid.Empty;

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
