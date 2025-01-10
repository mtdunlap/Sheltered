using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Api.Configuration.Databases;

/// <summary>
/// Extensions for regsitering a database to the service collection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    /// <summary>
    /// Asynchronously ensures the database for <typeparamref name="TContext"/> registered to <see cref="WebApplication"/> is created.
    /// </summary>
    /// <typeparam name="TContext">The <typeparamref name="TContext"/> for which the database will be created.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>true if the database for <typeparamref name="TContext"/> is created; otherwise false.</returns>
    public static async Task<bool> EnsureDatabaseCreatedAsync<TContext>(this WebApplication app, CancellationToken cancellationToken = default) where TContext : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<TContext>();
        return await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
