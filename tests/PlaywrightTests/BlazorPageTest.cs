using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

internal class BlazorPageTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = BlazorIntegrationTestSetUp.Host.ServerAddress,
        IgnoreHTTPSErrors = true
    };
}
