using System.Threading.Tasks;
using NUnit.Framework;
using WireMock.Server;

namespace Web.IntegrationTests;

[SetUpFixture]
internal sealed class BlazorHostSetUp
{
    public static BlazorApplicationFactory<Program> Host { get; private set; } = null!;

    public static WireMockServer MockApi { get; private set; } = null!;

    [OneTimeSetUp]
    public void HostSetUp()
    {
        MockApi = WireMockServer.Start();
        Host = new(MockApi.Port);
    }

    [OneTimeTearDown]
    public async Task HostTearDown()
    {
        MockApi.Stop();
        MockApi.Dispose();
        await Host.DisposeAsync();
    }
}
