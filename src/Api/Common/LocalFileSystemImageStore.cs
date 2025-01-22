using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Api.Animals;
using Api.Configuration.Options;

namespace Api.Common;

/// <summary>
/// Represents an <see cref="IImageStore"/> for saving to and deleting from the local filesystem.
/// </summary>
/// <param name="options">The configuration options for the <see cref="LocalFileSystemImageStore"/>.</param>
/// <param name="linkGenerator">
/// A <see cref="LinkGenerator"/> to use for getting the location of the image resource.
/// </param>
public sealed class LocalFileSystemImageStore(IOptions<LocalFileSystemImageStoreOptions> options,
    LinkGenerator linkGenerator) : IImageStore
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
    /// A <see cref="Task"/> that represents the asynchronous operation that returns a <see cref="StoredImageInfo"/>
    /// representing information about the stored image.
    /// </returns>
    public async Task<StoredImageInfo> SaveAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        var image = new MagickImage(formFile.OpenReadStream());
        var baseDirectory = Path.GetFullPath(_options.ImageDirectory);
        var fileName = $"{Guid.NewGuid()}.{image.Format}";
        var filepath = Path.Combine(baseDirectory, fileName);
        await using var fileStream = File.Open(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await image.WriteAsync(fileStream, cancellationToken);
        return new()
        {
            Location = new(filepath),
            ContentType = formFile.ContentType,
            Height = image.Height,
            Width = image.Width,
            FileSize = formFile.Length >= 0 ? (ulong)formFile.Length : throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Retrieves the image from the store.
    /// </summary>
    /// <param name="location">The location of the image.</param>
    /// <returns>A <see cref="Stream"/> containing the image.</returns>
    public Stream Get(string location)
    {
        const string fileScheme = "file://";
        if (location.StartsWith(fileScheme) == false)
        {
            throw new InvalidOperationException();
        }
        return File.Open(location.Replace(fileScheme, string.Empty), FileMode.Open, FileAccess.Read, FileShare.None);
    }

    /// <summary>
    /// Gets the location of the image from api. May be null.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> of the request.</param>
    /// <param name="routeValues">The route values used to generate the path.</param>
    /// <returns>The location of the image from the api.</returns>
    public string? GetLocation(HttpContext httpContext, object? routeValues)
    {
        var action = nameof(ImageController.Get);
        var controller = nameof(ImageController).Replace("Controller", string.Empty);
        return linkGenerator.GetUriByAction(httpContext, action, controller, routeValues);
    }
}
