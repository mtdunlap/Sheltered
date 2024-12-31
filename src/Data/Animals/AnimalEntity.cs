using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Core.Animals;

namespace Data.Animals;

/// <summary>
/// Represents an entity for an animal.
/// </summary>
[Table("animals", Schema = "sheltered")]
public sealed record class AnimalEntity
{
    /// <summary>
    /// Gets or inits the id of the animal.
    /// </summary>
    /// <remarks>
    /// The Id is not intended to be set/init by users, but is necessary so that EF Core can set the Id.
    /// </remarks>
    /// <value>The id of the animal.</value>
    [Key]
    [Required]
    [Column("id", TypeName = "TEXT")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id
    {
        get;

        [ExcludeFromCodeCoverage(Justification = "Cannot test a private initter.")]
        private init;
    } = Guid.Empty;

    /// <summary>
    /// Gets or sets the name of the animal. May be null.
    /// </summary>
    /// <value>The name of the animal.</value>
    [Required]
    [Length(1, 50)]
    [Column("name", TypeName = "TEXT")]
    public required string? Name { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    [Required]
    [Column("kind", TypeName = "TEXT")]
    public required AnimalKind Kind { get; set; }
}
