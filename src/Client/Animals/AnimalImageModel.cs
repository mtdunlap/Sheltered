using System.Text.Json.Serialization;

namespace Client.Animals;

/// <summary>
/// Represents an image for an animal.
/// </summary>
public sealed record class AnimalImageModel
{
    /// <summary>
    /// Gets or inits the location of the image.
    /// </summary>
    /// <value>The location of the image.</value>
    [JsonPropertyName("location")]
    public required string Location { get; init; }
}
