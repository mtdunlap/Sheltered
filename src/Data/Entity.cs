using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data;

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
    public Guid Id { get; private init; } = Guid.Empty;

    [Required]
    [Column("created", TypeName = "TEXT")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Created { get; private init; } = DateTime.MinValue;

    [Required]
    [Column("last_updated", TypeName = "TEXT")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastUpdated { get; private init; } = DateTime.MinValue;
}
