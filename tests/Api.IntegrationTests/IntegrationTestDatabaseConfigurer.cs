using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

internal abstract class IntegrationTestDatabaseConfigurer<TContext> where TContext : DbContext
{
    protected static void RemoveContext(IServiceCollection services)
    {
        var dbContext = services.SingleOrDefault(descriptor =>
        {
            return descriptor.ServiceType == typeof(TContext);
        }) ?? throw new InvalidOperationException($"No configured service for {nameof(TContext)} was found.");

        services.Remove(dbContext);
    }
}
