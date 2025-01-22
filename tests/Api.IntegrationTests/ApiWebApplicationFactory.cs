using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Api.Common;
using Data;
using Tests.Common.Services;

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

            services.ReplaceRequiredScopedService<IImageStore, IImageStore>(serviceProvider =>
            {
                var imageStore = Substitute.For<IImageStore>();

                imageStore
                    .SaveAsync(Arg.Any<IFormFile>(), Arg.Any<CancellationToken>())
                    .Returns(new StoredImageInfo
                    {
                        Location = new Uri("http://localhost/", UriKind.Absolute),
                        ContentType = "image/png",
                        Height = 900,
                        Width = 1600,
                        FileSize = 100_000
                    });

                imageStore
                    .When(substituteImageStore => substituteImageStore.Delete(Arg.Any<Uri>()))
                    .Do(callInfo => { });

                imageStore
                    .Get(Arg.Any<string>())
                    .Returns(new MemoryStream());

                imageStore
                    .GetLocation(Arg.Any<HttpContext>(), Arg.Any<object?>())
                    .Returns((string?)null);

                return imageStore;
            });
        });

        builder.UseEnvironment("Development");
    }
}
