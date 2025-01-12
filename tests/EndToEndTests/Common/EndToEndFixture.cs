using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Tests.Common.Containers.Api;
using Tests.Common.Containers.Web;

namespace EndToEndTests.Common;

internal abstract class EndToEndFixture : PageTest
{
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

    private INetwork _network = null!;
    protected ApiContainer _api = null!;
    protected WebContainer _web = null!;

    [OneTimeSetUp]
    public async Task ContainerSetUp()
    {
        _network = new NetworkBuilder().Build();
        _api = new ApiContainer(_network);
        _web = new WebContainer(_network, _api.SharedNetworkAddress);
        await Task.WhenAll(_api.BuildAsync(), _web.StartAsync());
    }

    [OneTimeTearDown]
    public async Task ContainerTearDown()
    {
        await _api.DisposeAsync();
        await _web.DisposeAsync();
        await _network.DisposeAsync();
    }

    [SetUp]
    public async Task TraceSetUp()
    {
        await StartTraceAsync();
    }

    [TearDown]
    public async Task TraceTearDown()
    {
        if (IsFailed)
        {
            await StopAndSaveTraceAsync(TracePath);
        }
        else
        {
            await StopTraceAsync();
        }
    }

    private async Task StartTraceAsync()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = TraceTitle,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    private async Task StopTraceAsync()
    {
        await Context.Tracing.StopAsync();
    }

    private async Task StopAndSaveTraceAsync(string path)
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = path,
        });
    }

    private static bool IsFailed
    {
        get
        {
            return TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Error
                || TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Failure;
        }
    }

    private string TracePath
    {
        get
        {
            return Path.Combine(TestContext.CurrentContext.WorkDirectory, TraceDirectoryName, TraceTitle);
        }
    }

    private const string TraceDirectoryName = "playwright-traces";

    private string TraceTitle
    {
        get
        {
            return string.Join('.',
                TestContext.CurrentContext.Test.ClassName,
                TestContext.CurrentContext.Test.Name,
                Browser.BrowserType.Name,
                Browser.Version
            );
        }
    }
}
