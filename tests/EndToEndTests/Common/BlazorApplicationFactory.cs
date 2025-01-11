using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Client;
using System.Net.Http;

namespace EndToEndTests.Common;

/// <summary>
/// Spins up the Blazor Web App referenced by <typeparamref name="TProgram"/>.
/// The app is available via <c>127.0.0.1</c> on a random free port chosen at start
/// up.
/// </summary>
/// <typeparam name="TProgram"></typeparam>
internal sealed class BlazorApplicationFactory<TProgram>(HttpClient apiClient) : WebApplicationFactory<TProgram> where TProgram : class
{
    private IHost host = null!;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var client = services.SingleOrDefault(descriptor =>
            {
                return descriptor.ServiceType == typeof(IShelteredClient);
            }) ?? throw new InvalidOperationException($"No configured service for {nameof(IShelteredClient)} was found.");
            services.Remove(client);

            services.AddSingleton<IShelteredClient>(new ShelteredClient(apiClient));
        });

        base.ConfigureWebHost(builder);

        // Setting port to 0 means that Kestrel will pick any free a port.
        builder.UseUrls("http://127.0.0.1:0");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host for TestServer now before we modify the builder to use Kestrel instead.
        var testHost = builder.Build();

        // Modify the host builder to use Kestrel instead of TestServer so we can listen on a real address.
        // configure and start the actual host using Kestrel.
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel();
        });

        // Create and start the Kestrel server before the test server,
        // otherwise due to the way the deferred host builder works
        // for minimal hosting, the server will not get "initialized
        // enough" for the address it is listening on to be available.
        // See https://github.com/dotnet/aspnetcore/issues/33846.
        host = builder.Build();
        host.Start();

        // Extract the selected dynamic port out of the Kestrel server
        // and assign it onto the client options for convenience so it
        // "just works" as otherwise it'll be the default http://localhost
        // URL, which won't route to the Kestrel-hosted HTTP server.
        var server = host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();
        ClientOptions.BaseAddress = addresses!.Addresses.Select(x => new Uri(x)).Last();

        // Return the host that uses TestServer, rather than the real one.
        // Otherwise the internals will complain about the host's server
        // not being an instance of the concrete type TestServer.
        // See https://github.com/dotnet/aspnetcore/pull/34702.
        testHost.Start();
        return testHost;
    }

    private void EnsureServer()
    {
        if (host is null)
        {
            // This forces WebApplicationFactory to bootstrap the server
            using var _ = CreateDefaultClient();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        host?.Dispose();
    }
}
