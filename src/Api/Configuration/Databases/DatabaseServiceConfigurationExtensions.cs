using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Api.Configuration.Databases;

/// <summary>
/// Extensions for regsitering a database to the service collection.
/// </summary>
public static class DatabaseServiceConfigurationExtensions
{
    /// <summary>
    /// Adds a <typeparamref name="TContext"/> to the <see cref="IServiceCollection"/> using the provided <see cref="DatabaseSettings"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of <see cref="DbContext"/> to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database will be added.</param>
    /// <param name="databaseSettings">The <see cref="DatabaseSettings"/> containing the configuration settings for the database.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : DbContext
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseSettings.DataSource,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        return services.AddDbContext<TContext>(options => options.UseSqlite(connectionString));
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
}
