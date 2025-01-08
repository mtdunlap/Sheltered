using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Common;

namespace PlaywrightTests.Animals;

[TestFixture]
[BrowserAutomationTest]
[Parallelizable(ParallelScope.Self)]
internal sealed class DashboardFixture : BlazorPageTest
{
    [Test]
    public async Task Should_have_the_correct_title()
    {
        var dashboardPage = new DashboardPage(Page);
        var response = await dashboardPage.GotoAsync();
        await Expect(Page).ToHaveTitleAsync("Animals Dashboard");
    }
}
