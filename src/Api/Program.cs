using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Configuration.Databases;
using Api.Configuration.Services;

namespace Api;

/// <summary>
/// The main program for running the API.
/// </summary>
/// <remarks>
/// Added to allow applying the <see cref="ExcludeFromCodeCoverageAttribute"/> to this file.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed partial class Program
{
    /// <summary>
    /// Asynchronously builds and runs the api, blocking the calling thread until host shutdown.
    /// </summary>
    /// <param name="args">The arguments to use when building the api.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.AddDatabases();
        builder.AddServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await app.EnsureDatabasesCreatedAsync();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
