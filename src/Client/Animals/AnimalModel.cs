using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Animals;

namespace Client.Animals;

/// <summary>
/// Represents a model for an Animal.
/// </summary>
public sealed record class AnimalModel
{
    /// <summary>
    /// Gets or inits the name of the animal. May be null.
    /// </summary>
    /// <value>The name of the animal.</value>
    [Required, MaxLength(50, ErrorMessage = "Must not be longer than 50 characters.")]
    public required string? Name { get; init; } = null;

    /// <summary>
    /// Gets or inits the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    [Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public required AnimalKind Kind { get; init; } = AnimalKind.Unspecified;
}
