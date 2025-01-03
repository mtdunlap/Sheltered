using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Benchmarks;

internal sealed class SqliteIntegrationTestDatabaseConfigurer<TContext> : IntegrationTestDatabaseConfigurer<TContext>,
    IIntegrationTestDatabaseConfigurer where TContext : DbContext
{
    private readonly Action<IServiceCollection> _configure;

    private SqliteIntegrationTestDatabaseConfigurer(Action<IServiceCollection> configure)
    {
        _configure = configure;
    }

    public void Configure(IServiceCollection services)
    {
        RemoveContext(services);
        _configure(services);
    }

    public static SqliteIntegrationTestDatabaseConfigurer<TContext> RandomlyNamedInMemoryConfigurer
         => new(ConfigureRandomlyNamedSqliteInMemoryDatabase);

    public static SqliteIntegrationTestDatabaseConfigurer<TContext> RandomlyNamedConfigurer
         => new(ConfigureRandomlyNamedSqliteDatabase);

    private static void ConfigureRandomlyNamedSqliteInMemoryDatabase(IServiceCollection services)
    {
        var id = Guid.NewGuid();
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = $"{id}",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Private
        };
        var connectionString = connectionStringBuilder.ToString();
        ConfigureSqliteInMemoryDatabase(services, connectionString);
    }

    private static void ConfigureSqliteInMemoryDatabase(IServiceCollection services, string connectionString)
    {
        //Create open SqliteConnection so EF won't automatically close it.
        services.AddSingleton<DbConnection>(container =>
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            return connection;
        });

        services.AddDbContext<TContext>((serviceProvider, options) =>
        {
            var connection = serviceProvider.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });
    }

    private static void ConfigureRandomlyNamedSqliteDatabase(IServiceCollection services)
    {
        var id = Guid.NewGuid();
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = $"{id}.db",
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Private
        };
        var connectionString = connectionStringBuilder.ToString();
        ConfigureSqliteDatabase(services, connectionString);
    }

    private static void ConfigureSqliteDatabase(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
    }
}
