using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.UnitTests;

/// <summary>
/// A fake <see cref="HttpMessageHandler"/> for unit testing with an <see cref="HttpClient"/>.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    /// <summary>
    /// The <see cref="HttpResponseMessage"/> with which to respond.
    /// </summary>
    public required HttpResponseMessage Response { get; init; }

    /// <summary>
    /// Returns the configured <see cref="HttpResponseMessage"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Response);
    }
}
