using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Client;

/// <summary>
/// An abstract base client for sending HTTP requests.
/// </summary>
/// <param name="httpClient">An <see cref="HttpClient"/> to use when sending HTTP requests.</param>
public abstract class ClientBase(HttpClient httpClient) : IDisposable
{
    /// <summary>
    /// Finalizes the <see cref="ClientBase"/> and disposes the underlying <see cref="HttpClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~ClientBase()
    {
        httpClient.Dispose();
    }

    /// <summary>
    /// Asynchronously sends an HTTP DELETE request to <paramref name="requestUri"/>.
    /// </summary>
    /// <param name="requestUri">The <see cref="Uri"/> to which the HTTP DELETE request will be sent.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task<bool> DeleteAsync(Uri requestUri, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(requestUri, cancellationToken);
        var statusCode = response.StatusCode;
        return statusCode switch
        {
            HttpStatusCode.NoContent => true,
            HttpStatusCode.NotFound => false,
            _ => throw new HttpRequestException(
                    $"{nameof(statusCode)} is neither {HttpStatusCode.NoContent} nor {HttpStatusCode.NotFound}.",
                    null,
                    statusCode)
        };
    }

    /// <summary>
    /// Asynchronously sends an HTTP HEAD request to <paramref name="requestUri"/>.
    /// </summary>
    /// <param name="requestUri">The <see cref="Uri"/> to which the HTTP HEAD request will be sent.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task<bool> HeadAsync(Uri requestUri, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage
        {
            Method = HttpMethod.Head,
            RequestUri = requestUri
        };
        using var response = await httpClient.SendAsync(request, cancellationToken);
        var statusCode = response.StatusCode;
        return statusCode switch
        {
            HttpStatusCode.NoContent => true,
            HttpStatusCode.NotFound => false,
            _ => throw new HttpRequestException(
                    $"{nameof(statusCode)} is neither {HttpStatusCode.NoContent} nor {HttpStatusCode.NotFound}.",
                    null,
                    statusCode)
        };
    }

    /// <summary>
    /// Asynchronously sends an HTTP GET request to <paramref name="requestUri"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model being requested.</typeparam>
    /// <param name="requestUri">The <see cref="Uri"/> to which the HTTP PUT request will be sent.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task<TModel?> GetFromJsonAsync<TModel>(Uri requestUri, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        var statusCode = response.StatusCode;
        if (statusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"{nameof(statusCode)} is not {HttpStatusCode.OK}.",
                null,
                statusCode);
        }
        return await response.Content.ReadFromJsonAsync<TModel>(cancellationToken);
    }

    /// <summary>
    /// Asynchronously sends an HTTP POST request to <paramref name="requestUri"/>
    /// with a json body representing <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model being created.</typeparam>
    /// <param name="requestUri">The <see cref="Uri"/> to which the HTTP PUT request will be sent.</param>
    /// <param name="model">The updated <typeparamref name="TModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="JsonException"></exception>
    protected async Task<(TModel? created, Uri? location)> PostAsJsonAsync<TModel>(Uri requestUri, TModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(requestUri, model, cancellationToken);
        var statusCode = response.StatusCode;
        if (statusCode != HttpStatusCode.Created)
        {
            throw new HttpRequestException(
                $"{nameof(statusCode)} is not {HttpStatusCode.Created}.",
                null,
                statusCode);
        }
        var created = await response.Content.ReadFromJsonAsync<TModel>(cancellationToken);
        return (created, response.Headers.Location);
    }

    /// <summary>
    /// Asynchronously sends an HTTP PUT request to <paramref name="requestUri"/>
    /// with a json body representing <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model being updated.</typeparam>
    /// <param name="requestUri">The <see cref="Uri"/> to which the HTTP PUT request will be sent.</param>
    /// <param name="model">The updated <typeparamref name="TModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task<bool> PutAsJsonAsync<TModel>(Uri requestUri, TModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(requestUri, model, cancellationToken);
        var statusCode = response.StatusCode;
        return statusCode switch
        {
            HttpStatusCode.NoContent => true,
            HttpStatusCode.NotFound => false,
            _ => throw new HttpRequestException(
                    $"{nameof(statusCode)} is neither {HttpStatusCode.NoContent} nor {HttpStatusCode.NotFound}.",
                    null,
                    statusCode)
        };
    }

    /// <summary>
    /// Disposes the underlying <see cref="HttpClient"/>.
    /// </summary>
    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
