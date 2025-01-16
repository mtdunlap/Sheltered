using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Api.Configuration.Options;

namespace Api.Common;

/// <summary>
/// Represents an <see cref="IImageStore"/> for saving to and deleting from the local filesystem.
/// </summary>
/// <param name="options">The configuration options for the <see cref="LocalFileSystemImageStore"/>.</param>
public sealed class LocalFileSystemImageStore(IOptions<LocalFileSystemImageStoreOptions> options) : IImageStore
{
    private readonly LocalFileSystemImageStoreOptions _options = options.Value;

    /// <summary>
    /// Deletes the image at the provided <paramref name="filepath"/>.
    /// </summary>
    /// <param name="filepath">A <see cref="Uri"/> representing the full filepath of the image to be deleted.</param>
    public void Delete(Uri filepath)
    {
        File.Delete(filepath.ToString());
    }

    /// <summary>
    /// Asynchronously saves an <see cref="IFormFile"/> image to the local file system with a random name.
    /// </summary>
    /// <param name="formFile">The <see cref="IFormFile"/> to save.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation that returns a <see cref="Uri"/> representing
    /// the location of the image.
    /// </returns>
    public async Task<Uri> SaveAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        formFile.ThrowIfNotImage();
        var fileName = $"{Guid.NewGuid()}.{formFile.GetImageFileExtension()}";
        var baseDirectory = Path.GetFullPath(_options.ImageDirectory);
        var filepath = Path.Combine(baseDirectory, fileName);
        await using var fileStream = File.Open(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await formFile.CopyToAsync(fileStream, cancellationToken);
        return new(filepath);
    }
}
