using System.Threading.Tasks;
using NUnit.Framework;

namespace Api.IntegrationTests;

[SetUpFixture]
internal sealed class ApiHostSetUp
{
    public static ApiWebApplicationFactory<Program> Host { get; private set; } = null!;

    [OneTimeSetUp]
    public void HostSetUp()
    {
        Host = new();
    }

    [OneTimeTearDown]
    public async Task HostTearDown()
    {
        await Host.DisposeAsync();
    }
}
