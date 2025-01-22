using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Common.Services;

/// <summary>
/// Extension methods for an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the required scoped <typeparamref name="TService"/> with <typeparamref name="TImplementation"/>.
    /// </summary>
    /// <typeparam name="TService">The service to replace.</typeparam>
    /// <typeparam name="TImplementation">The implementation to use instead.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static void ReplaceRequiredScopedService<TService, TImplementation>(this IServiceCollection services)
        where TService : class where TImplementation : class, TService
    {
        services.RemoveRequiredService<TService>();
        services.AddScoped<TService, TImplementation>();
    }

    /// <summary>
    /// Replaces the required scoped <typeparamref name="TService"/> with <typeparamref name="TImplementation"/>.
    /// </summary>
    /// <typeparam name="TService">The service to replace.</typeparam>
    /// <typeparam name="TImplementation">The implementation to use instead.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="implementationFactory">
    /// A <see cref="Func{IServiceProvider, TImplementation}"/> for creating a new instance of
    /// <typeparamref name="TImplementation"/>.
    /// </param>
    public static void ReplaceRequiredScopedService<TService, TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory) where TService : class
            where TImplementation : class, TService
    {
        services.RemoveRequiredService<TService>();
        services.AddScoped<TService, TImplementation>(implementationFactory);
    }

    /// <summary>
    /// Removes the required <typeparamref name="TService"/> from the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of the <see cref="ServiceDescriptor"/> to retrieve from the service collection.
    /// </typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static void RemoveRequiredService<TService>(this IServiceCollection services) where TService : class
    {
        var serivce = services.GetRequiredServiceDescriptor<TService>();
        services.Remove(serivce);
    }

    /// <summary>
    /// Gets a required <see cref="ServiceDescriptor"/> for <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of the <see cref="ServiceDescriptor"/> to retrieve from the service collection.
    /// </typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The found <see cref="ServiceDescriptor"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// No configured service for <typeparamref name="TService"/> was found.
    /// </exception>
    private static ServiceDescriptor GetRequiredServiceDescriptor<TService>(this IServiceCollection services)
        where TService : class
    {
        return services.SingleOrDefault(descriptor =>
        {
            return descriptor.ServiceType == typeof(TService);
        }) ?? throw new InvalidOperationException($"No configured service for {nameof(TService)} was found.");
    }
}
