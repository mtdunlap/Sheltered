using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Client;

public abstract class ApiClient : IDisposable
{
    private readonly ClientOptions _options;
    private readonly HttpClient _httpClient;

    protected ApiClient(HttpClient httpClient, IOptions<ClientOptions> options)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = _options.BaseAddress;
    }

    ~ApiClient()
    {
        _httpClient.Dispose();
    }

    protected async Task<bool> HeadAsync(Uri relativeRequestUri, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Head,
            RequestUri = relativeRequestUri
        };
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    protected async Task<bool> HeadAsync(string relativeRequestUrl, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri(relativeRequestUrl, UriKind.Relative);
        return await HeadAsync(relativeRequestUri, cancellationToken);
    }

    protected async Task<TModel> GetFromJsonAsync<TModel>(Uri relativeRequestUri, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(relativeRequestUri, cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var model = await response.Content.ReadFromJsonAsync<TModel>(cancellationToken);
            return model ?? throw new InvalidOperationException($"An error occurred when deserializing the json response to {nameof(TModel)}.");
        }
        throw new InvalidOperationException();
    }

    protected async Task<TModel> GetFromJsonAsync<TModel>(string relativeRequestUrl, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri(relativeRequestUrl, UriKind.Relative);
        return await GetFromJsonAsync<TModel>(relativeRequestUri, cancellationToken);
    }

    protected async Task<(TModel, Uri)> PostAsJsonAsync<TModel>(Uri relativeRequestUri, TModel model, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(relativeRequestUri, model, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            var location = response.Headers.Location ?? throw new InvalidOperationException();
            var created = await response.Content.ReadFromJsonAsync<TModel>(cancellationToken) ?? throw new InvalidOperationException();
            return (created, location);
        }
        throw new InvalidOperationException();
    }

    protected async Task<(TModel, Uri)> PostAsJsonAsync<TModel>(string relativeRequestUrl, TModel model, CancellationToken cancellationToken = default)
    {
        var relativeRequestUri = new Uri(relativeRequestUrl, UriKind.Relative);
        return await PostAsJsonAsync(relativeRequestUri, model, cancellationToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
