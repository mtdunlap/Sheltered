using System;

namespace Api.Common;

/// <summary>
/// Represents information about a stored image.
/// </summary>
public sealed class StoredImageInfo
{
    /// <summary>
    /// Gets or inits the location of the stored image. May be null.
    /// </summary>
    /// <value>The location of the stored image.</value>
    public required Uri Location { get; init; }

    /// <summary>
    /// Gets or inits the content type of the stored image.
    /// </summary>
    /// <value>The content type of the stored image.</value>
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets or inits the height of the stored image in pixels.
    /// </summary>
    /// <value>The height of the stored image in pixels.</value>
    public required uint Height { get; init; }

    /// <summary>
    /// Gets or inits the width of the stored image in pixels.
    /// </summary>
    /// <value>The width of the stored image in pixels.</value>
    public required uint Width { get; init; }

    /// <summary>
    /// Gets or inits the file size of the stored image in bytes.
    /// </summary>
    /// <value>The file size of the stored image in bytes.</value>
    public required ulong FileSize { get; init; }
}
