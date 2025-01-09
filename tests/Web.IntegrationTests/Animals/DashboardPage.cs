using System.Threading.Tasks;
using Client.Animals;
using Microsoft.Playwright;

namespace Web.IntegrationTests.Animals;

internal sealed class DashboardPage(IPage page)
{
    public const string Url = "animals";

    public async Task<IResponse?> GotoAsync()
    {
        return await page.GotoAsync(Url);
    }

    public async Task<IResponse?> GotoAsync(PageGotoOptions pageGotoOptions)
    {
        return await page.GotoAsync(Url, pageGotoOptions);
    }

    public ILocator AddAnimalButton => page.GetByRole(AriaRole.Button);

    public ILocator GetAnimalDetailLink(AnimalModel animalModel) => page.GetByRole(AriaRole.Link, new() { Name = $"Name {animalModel.Name} Kind {animalModel.Kind}" });
}
