using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Client;

namespace Web.Configuration.Http;

/// <summary>
/// Extensions for regsitering a typed <see cref="HttpClient"/> to the service collection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpClientExtensions
{
    /// <summary>
    /// Adds the typed <see cref="HttpClient"/>s to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>The same <see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.AddHttpClient<IShelteredClient, ShelteredClient>();
        return builder;
    }

    /// <summary>
    /// Adds a typed <see cref="HttpClient"/> to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <typeparam name="TClient">The interface for the actual typed <see cref="HttpClient"/>.</typeparam>
    /// <typeparam name="TImplementation">The implementation of the actual typed <see cref="HttpClient"/>.</typeparam>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>The same <see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder AddHttpClient<TClient, TImplementation>(this WebApplicationBuilder builder)
        where TClient : class where TImplementation : class, TClient
    {
        var httpClientSettings = builder.Configuration.GetHttpClientSettings();
        builder.Services.AddHttpClient<IShelteredClient, ShelteredClient>(httpClient =>
        {
            httpClient.BaseAddress = httpClientSettings.BaseAddress;
        });
        return builder;
    }

    /// <summary>
    /// Reads the settings configuration and returns the values as a <see cref="HttpClientSettings"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="ConfigurationManager"/>.</param>
    /// <returns>The <see cref="HttpClientSettings"/> containing the configuration settings for the database.</returns>
    /// <exception cref="InvalidOperationException">The http client settings are missing or malformed.</exception>
    public static HttpClientSettings GetHttpClientSettings(this ConfigurationManager configuration)
    {
        var httpClientSection = configuration.GetSection(HttpClientSettings.Key);
        return httpClientSection.Get<HttpClientSettings>()
            ?? throw new InvalidOperationException("The http client settings are missing or malformed. Please check the appSettings.[Environment].json file.");
    }
}
