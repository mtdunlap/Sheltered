using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Tests.Common.Containers.Api;

public sealed class ApiContainer : IAsyncDisposable
{
    private const string networkAlias = "api";

    private readonly INetwork _network;

    private readonly IContainer _apiContainer;

    private static readonly ApiImage Image = new();

    public ApiContainer() : this(new NetworkBuilder().Build()) { }

    public ApiContainer(INetwork network)
    {
        _network = network;
        _apiContainer = new ContainerBuilder()
            .WithImage(Image)
            .WithNetwork(_network)
            .WithPortBinding(ApiImage.HttpPort, true)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithEnvironment("Database__DataSource", "test.db")
            .WithEnvironment("Database__Provider", "sqlite")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ApiImage.HttpPort))
            .WithNetworkAliases(networkAlias)
            .Build();
    }

    public Uri BaseAddress
    {
        get
        {
            return new UriBuilder("http", _apiContainer.Name, _apiContainer.GetMappedPublicPort(ApiImage.HttpPort)).Uri;
        }
    }

    public Uri SharedNetworkAddress
    {
        get
        {
            return new UriBuilder("http", networkAlias, ApiImage.HttpPort).Uri;
        }
    }

    public async Task StartAsync()
    {
        await Image.StartAsync().ConfigureAwait(false);

        await _network.CreateAsync().ConfigureAwait(false);

        await _apiContainer.StartAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
        await Image.DisposeAsync().ConfigureAwait(false);

        await _apiContainer.DisposeAsync().ConfigureAwait(false);

        await _network.DeleteAsync().ConfigureAwait(false);
    }
}
