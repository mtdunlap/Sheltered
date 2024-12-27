using System;
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
public static class DatabaseExtensions
{
    public static WebApplicationBuilder AddDatabases(this WebApplicationBuilder builder)
    {
        builder.AddDatabase<ShelteredContext>();
        return builder;
    }

    public static WebApplicationBuilder AddDatabase<TContext>(this WebApplicationBuilder builder) where TContext : DbContext
    {
        var databaseOptions = builder.Configuration.GetDatabaseSettings();
        builder.Services.AddDatabase<TContext>(databaseOptions);
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
    /// <typeparam name="TContext">The type of <see cref="DbContext"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database will be added.</param>
    /// <param name="databaseSettings">The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : DbContext => databaseSettings.Provider switch
    {
        DatabaseProvider.SQLite => services.AddSqliteDatabase<TContext>(databaseSettings),
        _ => throw new InvalidOperationException()
    };

    /// <summary>
    /// Adds a <typeparamref name="TContext"/> to the <see cref="IServiceCollection"/> using the provided <see cref="DatabaseSettings"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of <see cref="DbContext"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database will be added.</param>
    /// <param name="databaseSettings">The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSqliteDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : DbContext
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseSettings.DataSource,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        return services.AddDbContext<TContext>(options => options.UseSqlite(connectionString));
    }
}
