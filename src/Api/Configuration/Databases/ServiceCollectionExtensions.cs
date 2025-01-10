using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Core.Animals;
using Data;

namespace Api.Configuration.Databases;

public static class ServiceCollectionExtensions
{
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
        DatabaseProvider.NpgSQL => services.AddPostgreDatabase<TContext>(databaseSettings),
        _ => throw new InvalidOperationException($"The provided {databaseSettings.Provider} is unknown or not supported. Please check the appSettings.[Environment].json file.")
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
            DataSource = databaseSettings.Database,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        return services.AddDbContext<TContext>(options => options.UseSqlite(connectionString));
    }

    public static IServiceCollection AddPostgreDatabase<TContext>(this IServiceCollection services, DatabaseSettings databaseSettings) where TContext : DbContext
    {
        if (databaseSettings.Port is null)
        {
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                Database = databaseSettings.Database,
                Host = databaseSettings.Host,
                Username = databaseSettings.Username,
                Password = databaseSettings.Password,
            }.ToString();
            return services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<AnimalKind>(ShelteredContext.AnimalKindEnumTypeName, ShelteredContext.Schema);
            }));
        }
        else
        {
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                Database = databaseSettings.Database,
                Host = databaseSettings.Host,
                Username = databaseSettings.Username,
                Password = databaseSettings.Password,
                Port = databaseSettings.Port.Value
            }.ToString();
            return services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<AnimalKind>(ShelteredContext.AnimalKindEnumTypeName, ShelteredContext.Schema);
            }));
        }
    }
}
