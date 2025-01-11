using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using EndToEndTests.Common;

namespace EndToEndTests.Animals;

[TestFixture]
[NonParallelizable]
internal sealed class CreateNewAnimalWorkflowFixture : BlazorPageFixture
{
    [Test]
    public async Task Should_create_a_new_animal_When_completing_the_new_animal_workflow()
    {
        Guid id = Guid.Empty;
        _ = await Page.GotoAsync("animals/new", new() { WaitUntil = WaitUntilState.NetworkIdle });

        await Page.GetByLabel("name").FillAsync("Lucy");
        await Page.GetByLabel("kind").SelectOptionAsync("Cat");
        await Page.RunAndWaitForResponseAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button).ClickAsync();
        }, response =>
        {
            return Guid.TryParse(response.Url.Split('/').Last(), out id);
        }, new() { Timeout = 5000 });

        Assert.Multiple(async () =>
        {
            await Expect(Page).ToHaveURLAsync($"animals/{id}", new() { Timeout = 5000 });
            await Page.Locator("span").Filter(new() { HasText = "Lucy" }).IsVisibleAsync();
            await Page.Locator("span").Filter(new() { HasText = "Cat" }).IsVisibleAsync();
        });
    }
}
