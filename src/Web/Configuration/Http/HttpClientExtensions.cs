using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Client;

namespace Web.Configuration.Http;

public static class HttpClientExtensions
{
    public static WebApplicationBuilder AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.AddHttpClient<IShelteredClient, ShelteredClient>();
        return builder;
    }

    public static WebApplicationBuilder AddHttpClient<TClient, TImplementation>(this WebApplicationBuilder builder) where TClient : class where TImplementation : class, TClient
    {
        var httpClientSettings = builder.Configuration.GetHttpClientSettings();
        builder.Services.AddHttpClient<IShelteredClient, ShelteredClient>(httpClient =>
        {
            httpClient.BaseAddress = httpClientSettings.BaseAddress;
        });
        return builder;
    }

    public static HttpClientSettings GetHttpClientSettings(this ConfigurationManager configuration)
    {
        var httpClientSection = configuration.GetSection(HttpClientSettings.Key);
        return httpClientSection.Get<HttpClientSettings>()
            ?? throw new InvalidOperationException("The http client settings are missing or malformed. Please check the appSettings.[Environment].json file.");
    }
}
