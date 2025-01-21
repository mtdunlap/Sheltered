using System.Text.Json.Serialization;
using Client.Common;

namespace Client.Animals;

/// <summary>
/// Represents an image for an animal.
/// </summary>
public sealed record class AnimalImageModel : Model
{
    /// <summary>
    /// Gets or inits the location of the image.
    /// </summary>
    /// <value>The location of the image.</value>
    [JsonPropertyName("location")]
    public required string Location { get; init; }

    /// <summary>
    /// Gets or inits the content type of the image.
    /// </summary>
    /// <value>The content type of the image.</value>
    [JsonPropertyName("contentType")]
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets or inits the height of the image in pixels.
    /// </summary>
    /// <value>The height of the image in pixels.</value>
    [JsonPropertyName("height")]
    public required uint Height { get; init; }

    /// <summary>
    /// Gets or inits the width of the image in pixels.
    /// </summary>
    /// <value>The width of the image in pixels.</value>
    [JsonPropertyName("width")]
    public required uint Width { get; init; }

    /// <summary>
    /// Gets or inits the file size of the image in bytes.
    /// </summary>
    /// <value>The file size of the image in bytes.</value>
    [JsonPropertyName("fileSize")]
    public required ulong FileSize { get; init; }
}
