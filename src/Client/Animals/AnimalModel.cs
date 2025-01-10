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
    /// <value>The name of the animal. If the animal does not have a name use null.</value>
    [Required(AllowEmptyStrings = true)]
    [MaxLength(50, ErrorMessage = "Name must be less than or equal to 50 characters.")]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets or inits the <see cref="AnimalKind"/> of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    [Required(ErrorMessage = "The animal kind was not within the range of accepted values.")]
    [JsonPropertyName("kind")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AnimalKind Kind { get; init; } = AnimalKind.Unspecified;
}
