using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Api.Configuration.Databases;

/// <summary>
/// Extensions for regsitering a database to the service collection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DatabaseExtensions
{
    /// <summary>
    /// Asynchronously ensures all the databases registered to <see cref="WebApplication"/> is created.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>true if all of the databases are created; otherwise false.</returns>
    public static async Task<bool> EnsureDatabasesCreatedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        return await app.EnsureDatabaseCreatedAsync<ShelteredContext>(cancellationToken);
    }

    /// <summary>
    /// Asynchronously ensures the database for <typeparamref name="TContext"/> registered to <see cref="WebApplication"/> is created.
    /// </summary>
    /// <typeparam name="TContext">The <typeparamref name="TContext"/> for which the database will be created.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>true if the database for <typeparamref name="TContext"/> is created; otherwise false.</returns>
    public static async Task<bool> EnsureDatabaseCreatedAsync<TContext>(this WebApplication app, CancellationToken cancellationToken = default) where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<TContext>();
        return await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    /// <summary>
    /// Adds the databases to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>The same <see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder AddDatabases(this WebApplicationBuilder builder)
    {
        var databaseOptions = builder.Configuration.GetDatabaseSettings();
        builder.Services.AddDatabase<ShelteredContext>(databaseOptions);
        return builder;
    }

    /// <summary>
    /// Reads the settings configuration and returns the values as a <see cref="DatabaseSettings"/>.
    /// </summary>
    /// <param name="configurationManager">The <see cref="ConfigurationManager"/>.</param>
    /// <returns>The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</returns>
    /// <exception cref="InvalidOperationException">The database settings are missing or malformed.</exception>
    public static DatabaseSettings GetDatabaseSettings(this ConfigurationManager configurationManager)
    {
        var databaseSection = configurationManager.GetSection(DatabaseSettings.Key);
        return databaseSection.Get<DatabaseSettings>()
            ?? throw new InvalidOperationException("The database settings are missing or malformed. Please check the appSettings.[Environment].json file.");
    }

    /// <summary>
    /// Adds a <typeparamref name="TContext"/> to the <see cref="IServiceCollection"/> using the provided <see cref="DatabaseSettings"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database will be added.</param>
    /// <param name="databaseSettings">The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : Microsoft.EntityFrameworkCore.DbContext => databaseSettings.Provider switch
    {
        DatabaseProvider.SQLite => services.AddSqliteDatabase<TContext>(databaseSettings),
        _ => throw new InvalidOperationException($"The provided {databaseSettings.Provider} is unknown or not supported. Please check the appSettings.[Environment].json file.")
    };

    /// <summary>
    /// Adds a <typeparamref name="TContext"/> to the <see cref="IServiceCollection"/> using the provided <see cref="DatabaseSettings"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database will be added.</param>
    /// <param name="databaseSettings">The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSqliteDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseSettings.DataSource,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        return services.AddDbContext<TContext>(options => options.UseSqlite(connectionString));
    }
}
