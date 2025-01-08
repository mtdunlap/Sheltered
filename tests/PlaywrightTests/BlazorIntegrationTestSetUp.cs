using System.Threading.Tasks;
using NUnit.Framework;

namespace PlaywrightTests;

[SetUpFixture]
internal sealed class BlazorIntegrationTestSetUp
{
    public static BlazorApplicationFactory<Program> Host { get; private set; } = null!;

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
