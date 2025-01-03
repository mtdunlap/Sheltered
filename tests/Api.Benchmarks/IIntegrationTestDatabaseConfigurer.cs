using Microsoft.Extensions.DependencyInjection;

namespace Api.Benchmarks;

internal interface IIntegrationTestDatabaseConfigurer
{
    void Configure(IServiceCollection services);
}
