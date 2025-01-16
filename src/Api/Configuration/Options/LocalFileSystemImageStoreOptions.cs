using System.ComponentModel.DataAnnotations;
using Api.Common;

namespace Api.Configuration.Options;

/// <summary>
/// Represents configuration options for the <see cref="LocalFileSystemImageStore"/>
/// </summary>
public sealed record class LocalFileSystemImageStoreOptions
{
    /// <summary>
    /// The key for the options group.
    /// </summary>
    public const string SectionKey = "Images";

    /// <summary>
    /// The base image directory to save all images to.
    /// </summary>
    [Required]
    public required string ImageDirectory { get; init; }
}
