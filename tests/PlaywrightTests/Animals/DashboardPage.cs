using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Animals;

internal sealed class DashboardPage(IPage page)
{
    public async Task<IResponse?> GotoAsync()
    {
        return await page.GotoAsync("animals");
    }
}
