using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Components;
using Web.Configuration.Http;

namespace Web;

/// <summary>
/// The main program for running the Web project.
/// </summary>
/// <remarks>
/// Added to allow applying the <see cref="ExcludeFromCodeCoverageAttribute"/> to this file.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class Program
{
    /// <summary>
    /// Builds and runs the web server and blocks the calling thread until host shutdown.
    /// </summary>
    /// <param name="args">The arguments to use when building the web server.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.AddHttpClients();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();


        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
