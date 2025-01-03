using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Client;
using Data;

namespace Api.IntegrationTests;

internal sealed class IntegrationTestWebApplicationFactory<TProgram>(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public ShelteredClient CreateShelteredClient()
    {
        var httpClient = CreateClient();
        return new(httpClient);
    }

    public TContext GetDbContext<TContext>() where TContext : DbContext
    {
        var scope = Server.Services.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<TContext>();
    }

    public async Task EnsureDatabasesDeletedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseDeletedAsync<ShelteredContext>(cancellationToken);
    }

    private async Task EnsureDatabaseDeletedAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        await using var scope = Server.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(databaseConfigurer.Configure);
        builder.UseEnvironment("Development");
    }

    public override async ValueTask DisposeAsync()
    {
        await EnsureDatabasesDeletedAsync();
        await base.DisposeAsync();
    }
}
