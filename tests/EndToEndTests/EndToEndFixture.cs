using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Tests.Common.Containers.Api;
using Tests.Common.Containers.Web;

namespace EndToEndTests;

internal abstract class EndToEndFixture : PageTest
{
    private INetwork _network = null!;
    protected ApiContainer _api = null!;
    protected WebContainer _web = null!;

    [OneTimeSetUp]
    public async Task ContainerSetUp()
    {
        _network = new NetworkBuilder().Build();
        _api = new ApiContainer(_network);
        _web = new WebContainer(_network, _api.SharedNetworkAddress);
        await Task.WhenAll(_api.StartAsync(), _web.StartAsync());
    }

    [OneTimeTearDown]
    public async Task ContainerTearDown()
    {
        await _api.DisposeAsync();
        await _web.DisposeAsync();
        await _network.DisposeAsync();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new()
        {
            BaseURL = _web.BaseAddress.ToString(),
            IgnoreHTTPSErrors = true,
            Locale = "en-US",
            ColorScheme = ColorScheme.Dark
        };
    }
}
