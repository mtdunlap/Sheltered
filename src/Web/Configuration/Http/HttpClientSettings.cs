using System;

namespace Web.Configuration.Http;

public sealed record class HttpClientSettings
{
    public const string Key = "Client";

    public required string BaseUrl { get; init; }

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
