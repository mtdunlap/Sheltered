using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Web.IntegrationTests.Animals;

internal sealed class NewAnimalPage(IPage page)
{
    public const string Url = "animals/new";

    public async Task<IResponse?> GotoAsync()
    {
        return await GotoAsync(new() { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task<IResponse?> GotoAsync(PageGotoOptions pageGotoOptions)
    {
        return await page.GotoAsync(Url, pageGotoOptions);
    }
}
