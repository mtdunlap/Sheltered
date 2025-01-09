using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using WireMock.Server;

namespace Web.IntegrationTests;

internal abstract class BlazorPageFixture : PageTest
{
    public static WireMockServer MockApi => BlazorHostSetUp.MockApi;

    [TearDown]
    public void MockApiReset()
    {
        MockApi.Reset();
    }

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = BlazorHostSetUp.Host.ServerAddress,
        IgnoreHTTPSErrors = true,
    };
}
