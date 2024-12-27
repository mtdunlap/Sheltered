using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Api.Animals;
using Data;

namespace Api.Configuration.Services;

/// <summary>
/// Extensions for adding services to the service collection.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds the required services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>The same <see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScopedServices();
        return builder;
    }

    /// <summary>
    /// Adds the required services to the <see cref="IServiceCollection"/> using the scoped service lifetime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IShelteredRepository, ShelteredRepository>();
        services.AddScoped<IAnimalMapper, AnimalMapper>();
        return services;
    }
}
