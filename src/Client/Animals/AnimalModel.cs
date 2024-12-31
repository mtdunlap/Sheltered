using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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
    /// <value>The name of the animal. If the animal does not have a name use null.</value>
    [Required(AllowEmptyStrings = false, ErrorMessage = "A name is required and cannot be empty, it may however be null.")]
    [Length(1, 50, ErrorMessage = "Name must be between 1 and 50 characters, inclusive.")]
    [JsonPropertyName("name")]
    public required string? Name { get; init; } = null;

    /// <summary>
    /// Gets or inits the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The animal kind was not within the range of accepted values.")]
    [JsonPropertyName("kind")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AnimalKind Kind { get; init; } = AnimalKind.Unspecified;
}
