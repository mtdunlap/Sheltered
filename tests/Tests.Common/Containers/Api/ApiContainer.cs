using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Tests.Common.Containers.Api;

/// <summary>
/// A test container for running the api.
/// </summary>
public sealed class ApiContainer : IAsyncDisposable
{
    private const string networkAlias = "api";

    private readonly INetwork _network;

    private readonly IContainer _apiContainer;

    private static readonly ApiImage Image = new();

    /// <summary>
    /// Initliazes a new instance of the api container using a default network.
    /// </summary>
    public ApiContainer() : this(new NetworkBuilder().Build()) { }

    /// <summary>
    /// Initliazes a new instance of the api container using the provided network.
    /// </summary>
    /// <param name="network">The network to which the api container should be attached.</param>
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

    /// <summary>
    /// A base <see cref="Uri"/> for accessing the running api container from the local system.
    /// </summary>
    public Uri BaseAddress
    {
        get
        {
            return new UriBuilder("http", _apiContainer.Name, _apiContainer.GetMappedPublicPort(ApiImage.HttpPort)).Uri;
        }
    }

    /// <summary>
    /// A base <see cref="Uri"/> for accessing the api container from another container on the same network.
    /// </summary>
    public static Uri SharedNetworkAddress
    {
        get
        {
            return new UriBuilder("http", networkAlias, ApiImage.HttpPort).Uri;
        }
    }

    /// <summary>
    /// Asynchronously build the api image, creates the network, and runs the api container.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task BuildAsync()
    {
        await Image.StartAsync().ConfigureAwait(false);

        await _network.CreateAsync().ConfigureAwait(false);

        await _apiContainer.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
        await Image.DisposeAsync().ConfigureAwait(false);

        await _apiContainer.DisposeAsync().ConfigureAwait(false);

        await _network.DeleteAsync().ConfigureAwait(false);
    }
}
