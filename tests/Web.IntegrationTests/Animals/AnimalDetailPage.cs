using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Web.IntegrationTests.Animals;

internal sealed class AnimalDetailPage(IPage page, Guid id)
{
    public string Url { get; } = GetRelativeUrl(id);

    public static string GetRelativeUrl(Guid id) => $"animals/{id}";

    public async Task<IResponse?> GotoAsync()
    {
        return await page.GotoAsync(Url);
    }

    public async Task<IResponse?> GotoAsync(PageGotoOptions pageGotoOptions)
    {
        return await page.GotoAsync(Url, pageGotoOptions);
    }
}
