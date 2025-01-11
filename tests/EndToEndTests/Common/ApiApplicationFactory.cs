using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Data;

namespace EndToEndTests.Common;

internal sealed class ApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
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

    private const string databaseName = "EndToEndTestDatabase";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(descriptor =>
            {
                return descriptor.ServiceType == typeof(ShelteredContext);
            }) ?? throw new InvalidOperationException($"No configured service for {nameof(ShelteredContext)} was found.");
            services.Remove(dbContext);

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = databaseName,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Private
            };
            var connectionString = connectionStringBuilder.ToString();

            //Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();

                return connection;
            });

            services.AddDbContext<ShelteredContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        base.ConfigureWebHost(builder);

        // Setting port to 0 means that Kestrel will pick any free a port.
        builder.UseUrls("http://127.0.0.1:0");

        builder.UseEnvironment("EndToEndTests");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            var configuration = new ConfigurationBuilder().Build();

            config.AddConfiguration(configuration);

            config.Sources.Add(new MemoryConfigurationSource
            {
                InitialData = new Dictionary<string, string>
                {
                    { "Database:DataSource", databaseName },
                    { "Database:Provider", "sqlite" }
                }!
            });
        });

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
