using System;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data;

namespace Api.IntegrationTests;

internal sealed class ApiWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var id = Guid.NewGuid();
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = $"{id}",
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

        builder.UseEnvironment("Development");
    }
}
