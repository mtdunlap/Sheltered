using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

internal interface IIntegrationTestDatabaseConfigurer
{
    void Configure(IServiceCollection services);
}
