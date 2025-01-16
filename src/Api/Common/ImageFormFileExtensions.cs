using System;
using Microsoft.AspNetCore.Http;

namespace Api.Common;

/// <summary>
/// Extensions for working with <see cref="IFormFile"/>s.
/// </summary>
public static class ImageFormFileExtensions
{
    private const string ImageContentTypeIdentifier = "image/";

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the content type of <paramref name="formFile"/> is
    /// not an image.
    /// </summary>
    /// <param name="formFile">The <see cref="IFormFile"/> on which the content type is being checked.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ThrowIfNotImage(this IFormFile formFile)
    {
        if (formFile.ContentType.StartsWith(ImageContentTypeIdentifier) == false)
        {
            throw new InvalidOperationException($"The content type must be an image, not {formFile.ContentType}.");
        }
    }

    /// <summary>
    /// Gets the file extension of the <paramref name="formFile"/> image.
    /// </summary>
    /// <param name="formFile">The <see cref="IFormFile"/>.</param>
    /// <returns>The file extension, excluding the dot.</returns>
    public static string GetImageFileExtension(this IFormFile formFile)
    {
        return formFile.ContentType.Replace(ImageContentTypeIdentifier, string.Empty);
    }
}
