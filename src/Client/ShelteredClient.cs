using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Client.Animals;
using Microsoft.Extensions.Options;

namespace Client;

public interface IShelteredClient : IDisposable
{
    Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AnimalModel> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(AnimalModel, Guid)> CreateAnimalAsync(AnimalModel animalModel, CancellationToken cancellationToken = default);
}

public sealed class ShelteredClient(HttpClient httpClient, IOptions<ClientOptions> options) : ApiClient(httpClient, options), IShelteredClient
{
    public async Task<bool> AnimalExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri($"animal/{id}", UriKind.Relative);
        return await HeadAsync(relativeRequestUri, cancellationToken);
    }

    public async Task<AnimalModel> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri($"animal/{id}", UriKind.Relative);
        return await GetFromJsonAsync<AnimalModel>(relativeRequestUri, cancellationToken);
    }

    public async Task<(AnimalModel, Guid)> CreateAnimalAsync(AnimalModel animalModel, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri($"animal/", UriKind.Relative);
        var (created, location) = await PostAsJsonAsync(relativeRequestUri, animalModel, cancellationToken);
        var parsed = Guid.TryParse(location.Segments[^1], out var id);
        if (parsed)
        {
            return (created, id);
        }
        throw new InvalidOperationException();
    }
}
