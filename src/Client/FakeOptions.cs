using Microsoft.Extensions.Options;

namespace Client;

public sealed class FakeOptions : IOptions<ClientOptions>
{
    public ClientOptions Value => new()
    {
        BaseUrl = "http://localhost:5108/"
    };
}
