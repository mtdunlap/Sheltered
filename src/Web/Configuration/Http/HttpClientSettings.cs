using System;
using System.Diagnostics.CodeAnalysis;

namespace Web.Configuration.Http;

/// <summary>
/// Represents the http client configuration settings.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record class HttpClientSettings
{
    /// <summary>
    /// The key for the http client settings.
    /// </summary>
    public const string Key = "Client";

    /// <summary>
    /// Gets or inits the base url for the http client.
    /// </summary>
    /// <value>The base url.</value>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Builds a <see cref="Uri"/> from the <see cref="BaseUrl"/> representing the base address for the http client.
    /// </summary>
    public Uri BaseAddress
    {
        get
        {
            if (BaseUrl.EndsWith("/", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                return new Uri(BaseUrl + '/', UriKind.Absolute);
            }
            return new Uri(BaseUrl, UriKind.Absolute);
        }
    }
}
