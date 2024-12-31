using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Client.Animals;

namespace Client;

/// <summary>
/// Defines a client for creating, reading, updating, deleting, and checking for the existence of data from the
/// sheltered api.
/// </summary>
public interface IShelteredClient : IDisposable
{
    /// <summary>
    /// Asynchronously determines if an <see cref="AnimalModel"/> with the provided <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the <see cref="AnimalModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates a new<see cref="AnimalModel"/>.
    /// </summary>
    /// <param name="animalModel">An <see cref="AnimalModel"/> representing the animal to create.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<(AnimalModel model, Guid id)> CreateAnimalAsync(AnimalModel animalModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes an <see cref="AnimalModel"/> with the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the <see cref="AnimalModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> DeleteAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously gets an <see cref="AnimalModel"/> with the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the <see cref="AnimalModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AnimalModel> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an <see cref="AnimalModel"/> with the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of the <see cref="AnimalModel"/>.</param>
    /// <param name="animalModel"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> UpdateAnimalByIdAsync(Guid id, AnimalModel animalModel, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a client for creating, reading, updating, deleting, and checking for the existence of data from the
/// sheltered api.
/// </summary>
/// <param name="httpClient">An <see cref="HttpClient"/> to use when sending HTTP requests.</param>
public sealed class ShelteredClient(HttpClient httpClient) : ClientBase(httpClient), IShelteredClient
{
    /// <summary>
    /// A relative url representing the animals address.
    /// </summary>
    private const string RelativeAnimalRequestBaseUrl = "animals";

    /// <summary>
    /// A relative <see cref="Uri"/> representing the animals address.
    /// </summary>
    private static readonly Uri RelativeAnimalRequestBaseAddress = new(RelativeAnimalRequestBaseUrl, UriKind.Relative);

    /// <summary>
    /// Builds a relative <see cref="Uri"/> representing the animals addresses.
    /// </summary>
    /// <param name="id">A <see cref="Guid"/> representing the id of <see cref="AnimalModel"/>. May be null.</param>
    /// <returns>A relative <see cref="Uri"/> representing the relative address of the animals endpoints.</returns>
    private static Uri RelativeAnimalRequestWithIdBaseAddress(Guid id)
    {
        return new($"{RelativeAnimalRequestBaseUrl}/{id}", UriKind.Relative);
    }

    /// <inheritdoc/>
    public async Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var requestAddress = RelativeAnimalRequestWithIdBaseAddress(id);
        return await HeadAsync(requestAddress, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(AnimalModel model, Guid id)> CreateAnimalAsync(AnimalModel animalModel,
        CancellationToken cancellationToken = default)
    {
        var (created, location) = await PostAsJsonAsync(RelativeAnimalRequestBaseAddress, animalModel, cancellationToken);
        var lastSegment = location?.Segments[^1] ?? throw new HttpRequestException("The response did not include a location header.");
        var guidText = lastSegment.EndsWith('/') ? lastSegment[..^1] : lastSegment;
        var parsed = Guid.TryParse(guidText, out var id);
        if (parsed)
        {
            return created is not null ? (created, id) : throw new HttpRequestException($"The deserialized json {nameof(AnimalModel)} was null.");
        }
        throw new HttpRequestException("The location header did not include an id.");
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var requestAddress = RelativeAnimalRequestWithIdBaseAddress(id);
        return await DeleteAsync(requestAddress, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<AnimalModel> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var requestAddress = RelativeAnimalRequestWithIdBaseAddress(id);
        return await GetFromJsonAsync<AnimalModel>(requestAddress, cancellationToken)
            ?? throw new HttpRequestException($"The deserialized json {nameof(AnimalModel)} was null.");
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAnimalByIdAsync(Guid id, AnimalModel animalModel,
        CancellationToken cancellationToken = default)
    {
        var requestAddress = RelativeAnimalRequestWithIdBaseAddress(id);
        return await PutAsJsonAsync(requestAddress, animalModel, cancellationToken);
    }
}
