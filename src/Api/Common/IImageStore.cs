using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Common;

/// <summary>
/// Provides a mechanism for asynchronously storing an <see cref="IFormFile"/> image and deleting an image.
/// </summary>
public interface IImageStore
{
    /// <summary>
    /// Deletes the image at the provided <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">A <see cref="Uri"/> representing the location of the image to delete.</param>
    void Delete(Uri uri);

    /// <summary>
    /// Asynchronously saves an <see cref="IFormFile"/> image.
    /// </summary>
    /// <param name="formFile">The <see cref="IFormFile"/> to save.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation that returns a <see cref="StoredImageInfo"/>
    /// representing information about the stored image.
    /// </returns>
    Task<StoredImageInfo> SaveAsync(IFormFile formFile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the image from the store.
    /// </summary>
    /// <param name="location">The location of the image.</param>
    /// <returns>A <see cref="Stream"/> containing the image.</returns>
    Stream Get(string location);

    /// <summary>
    /// Gets the location of the image from api. May be null.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> of the request.</param>
    /// <param name="routeValues">The route values used to generate the path.</param>
    /// <returns>The location of the image from the api.</returns>
    string? GetLocation(HttpContext httpContext, object? routeValues);
}
