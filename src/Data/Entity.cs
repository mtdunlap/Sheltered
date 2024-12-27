using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data;

/// <summary>
/// Represents a base entity.
/// </summary>
public abstract record class Entity
{
    /// <summary>
    /// Gets or inits the <see cref="Entity"/>'s id.
    /// </summary>
    /// <value>The id of the <see cref="Entity"/>.</value>
    [Key]
    [Required]
    [Column("id", TypeName = "TEXT")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private init; }

    /// <summary>
    /// Gets or inits the created <see cref="DateTime"/>.
    /// </summary>
    /// <value>A <see cref="DateTime"/> representing when the <see cref="Entity"/> was created.</value>
    [Required]
    [Column("created", TypeName = "REAL")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Created { get; private init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last updated <see cref="DateTime"/>.
    /// </summary>
    /// <remarks>
    /// Values that are updated on the database cannot have value conversions. See <see href="https://github.com/dotnet/efcore/issues/6999"/>.
    /// </remarks>
    /// <value>A <see cref="DateTime"/> representing when the <see cref="Entity"/> was last updated.</value>
    [Required]
    [Column("last_updated", TypeName = "REAL")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
