using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Tests.Common.Containers.Web;

public sealed class WebContainer : IAsyncDisposable
{
    private const string networkAlias = "web";

    private readonly INetwork _network;

    private readonly IContainer _webContainer;

    private static readonly WebImage Image = new();

    public WebContainer(INetwork network, Uri clientBaseAddress)
    {
        _network = network;
        _webContainer = new ContainerBuilder()
            .WithImage(Image)
            .WithNetwork(_network)
            .WithPortBinding(WebImage.HttpPort, true)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithEnvironment("Client__BaseUrl", $"{clientBaseAddress}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(WebImage.HttpPort))
            .WithNetworkAliases(networkAlias)
            .Build();
    }

    public Uri BaseAddress
    {
        get
        {
            return new UriBuilder("http", _webContainer.Hostname, _webContainer.GetMappedPublicPort(WebImage.HttpPort)).Uri;
        }
    }

    public Uri SharedNetworkAddress
    {
        get
        {
            return new UriBuilder("http", networkAlias, WebImage.HttpPort).Uri;
        }
    }

    public async Task StartAsync()
    {
        await Image.StartAsync().ConfigureAwait(false);

        await _network.CreateAsync().ConfigureAwait(false);

        await _webContainer.StartAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
        await Image.DisposeAsync().ConfigureAwait(false);

        await _webContainer.DisposeAsync().ConfigureAwait(false);

        await _network.DeleteAsync().ConfigureAwait(false);
    }
}
