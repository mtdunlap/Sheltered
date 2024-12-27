using System;
using System.ComponentModel.DataAnnotations;

namespace Client;

public sealed class ClientOptions
{
    public const string Client = "Client";

    [Required]
    public required string BaseUrl { get; init; }

    public Uri BaseAddress
    {
        get
        {
            if (BaseUrl.EndsWith('/'))
            {
                return new(BaseUrl, UriKind.Absolute);
            }
            return new(BaseUrl + '/', UriKind.Absolute);
        }
    }
}
