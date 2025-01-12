using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Tests.Common.Containers.Web;

/// <summary>
/// A test container for running the web server.
/// </summary>
public sealed class WebContainer : IAsyncDisposable
{
    private const string networkAlias = "web";

    private readonly INetwork _network;

    private readonly IContainer _webContainer;

    private static readonly WebImage Image = new();

    /// <summary>
    /// Initliazes a new instance of the web container using the provided network, and client base address.
    /// </summary>
    /// <param name="network">The network to which the web container should be attached.</param>
    /// <param name="clientBaseAddress">The base <see cref="Uri"/> to use for sending requests to the api container.</param>
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

    /// <summary>
    /// A base <see cref="Uri"/> for accessing the running web container from the local system.
    /// </summary>
    public Uri BaseAddress
    {
        get
        {
            return new UriBuilder("http", _webContainer.Hostname, _webContainer.GetMappedPublicPort(WebImage.HttpPort)).Uri;
        }
    }

    /// <summary>
    /// A base <see cref="Uri"/> for accessing the web container from another container on the same network.
    /// </summary>
    public static Uri SharedNetworkAddress
    {
        get
        {
            return new UriBuilder("http", networkAlias, WebImage.HttpPort).Uri;
        }
    }

    /// <summary>
    /// Asynchronously builds the web image, creates the network, and runs the web container.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync()
    {
        await Image.StartAsync().ConfigureAwait(false);

        await _network.CreateAsync().ConfigureAwait(false);

        await _webContainer.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
        await Image.DisposeAsync().ConfigureAwait(false);

        await _webContainer.DisposeAsync().ConfigureAwait(false);

        await _network.DeleteAsync().ConfigureAwait(false);
    }
}
