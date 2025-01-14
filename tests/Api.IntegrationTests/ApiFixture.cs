using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Client;
using Data;

namespace Api.IntegrationTests;

internal abstract class ApiFixture
{
    protected static ShelteredClient CreateShelteredClient()
    {
        var httpClient = ApiHostSetUp.Host.CreateClient();
        return new(httpClient);
    }

    protected static TContext GetDbContext<TContext>() where TContext : DbContext
    {
        var scope = ApiHostSetUp.Host.Services.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<TContext>();
    }

    [SetUp]
    public async Task DatabaseSetUp()
    {
        await using var scope = ApiHostSetUp.Host.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ShelteredContext>();
        await context.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public async Task DatabaseTearDown()
    {
        await using var scope = ApiHostSetUp.Host.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ShelteredContext>();
        await context.Database.EnsureDeletedAsync();
    }
}
