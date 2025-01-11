using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Data;

namespace EndToEndTests.Common;

internal abstract class BlazorPageFixture : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = EndToEndHostSetUp.Web.ServerAddress,
        IgnoreHTTPSErrors = true,
        Locale = "en-US",
        ColorScheme = ColorScheme.Dark,
    };

    [SetUp]
    public async Task BlazorPageSetUp()
    {
        await StartTraceAsync();
        await EnsureDatabaseCreatedAsync();
    }

    [TearDown]
    public async Task BlazorPageTearDown()
    {
        await EnsureDatabaseDeletedAsync();
        if (IsFailed)
        {
            await StopAndSaveTraceAsync(TracePath);
        }
        else
        {
            await StopTraceAsync();
        }
    }

    private static async Task EnsureDatabaseCreatedAsync()
    {
        await using var scope = EndToEndHostSetUp.Api.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ShelteredContext>();
        await context.Database.EnsureCreatedAsync();
    }

    private static async Task EnsureDatabaseDeletedAsync()
    {
        await using var scope = EndToEndHostSetUp.Api.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ShelteredContext>();
        await context.Database.EnsureDeletedAsync();
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
                Browser.Version,
                "zip"
            );
        }
    }
}
