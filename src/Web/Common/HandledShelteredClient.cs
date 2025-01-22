using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Client;
using Client.Animals;

namespace Web.Common;

/// <summary>
/// Provides a mechanism for sending and retrieving data to and from the sheltered api.
/// </summary>
public interface IHandledShelteredClient : IDisposable
{
    /// <summary>
    /// Asynchronously adds an image for an animal, optionally invoking an action on success or error.
    /// </summary>
    /// <param name="animalId">The id of the animal to which the image belongs.</param>
    /// <param name="file">The uploaded file.</param>
    /// <param name="maxAllowedSize">The maximum allowed file size.</param>
    /// <param name="onSuccess">
    /// An <see cref="Action{AnimalImageModel}"/> to invoke when the request completes successfully.
    /// </param>
    /// <param name="onError">An <see cref="Action{Exception}"/> to invoke when the request encounters an error.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task TryAddImageAsync(Guid animalId, IBrowserFile file, long maxAllowedSize = 512_000,
        Action<AnimalImageModel>? onSuccess = null, Action<Exception>? onError = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// A client for sending and retrieving data to and from the sheltered api.
/// </summary>
/// <param name="shelteredClient">The underlying <see cref="IShelteredClient"/> to use.</param>
public sealed class HandledShelteredClient(IShelteredClient shelteredClient) : IHandledShelteredClient
{
    /// <summary>
    /// Finalizes the <see cref="HandledShelteredClient"/>, disposing the <see cref="IShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~HandledShelteredClient()
    {
        shelteredClient.Dispose();
    }

    /// <inheritdoc cref="IHandledShelteredClient.TryAddImageAsync(Guid, IBrowserFile, long, Action{AnimalImageModel}?,
    ///     Action{Exception}?, CancellationToken)"/>
    public async Task TryAddImageAsync(Guid animalId, IBrowserFile file, long maxAllowedSize = 512_000,
        Action<AnimalImageModel>? onSuccess = null, Action<Exception>? onError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = file.OpenReadStream(maxAllowedSize, cancellationToken);
            var animalImageModel = await shelteredClient.AddImageAsync(animalId, stream, file.ContentType,
                cancellationToken);
            onSuccess?.Invoke(animalImageModel);
        }
        catch (Exception exception)
        {
            onError?.Invoke(exception);
        }
    }

    /// <summary>
    /// Disposes the <see cref="IShelteredClient"/>.
    /// </summary>
    public void Dispose()
    {
        shelteredClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
