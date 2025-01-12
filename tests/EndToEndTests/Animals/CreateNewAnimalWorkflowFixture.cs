using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Core.Animals;
using EndToEndTests.Common;

namespace EndToEndTests.Animals;

[TestFixture]
[TestFixtureSource(nameof(AnimalSource))]
[NonParallelizable]
internal sealed class CreateNewAnimalWorkflowFixture(string name, AnimalKind kind) : EndToEndFixture
{
    private static IEnumerable<TestFixtureData> AnimalSource()
    {
        yield return new TestFixtureData("Lucy", AnimalKind.Cat);
        yield return new TestFixtureData("Jake", AnimalKind.Dog);
    }

    private Guid _id = Guid.Empty;

    [Test, Order(1)]
    public async Task Should_create_a_new_animal()
    {
        _ = await Page.GotoAsync("animals/new", new() { WaitUntil = WaitUntilState.NetworkIdle });

        await Page.GetByLabel("name").FillAsync(name);
        await Page.GetByLabel("kind").SelectOptionAsync(kind.ToString());

        await Page.RunAndWaitForResponseAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button).ClickAsync();
        }, response =>
        {
            var success = Guid.TryParse(response.Url.Split('/').Last(), out var guid);
            if (success)
            {
                _id = guid;
            }
            return success;
        });

        await Expect(Page).ToHaveURLAsync($"animals/{_id}");
    }

    [Test, Order(2)]
    public async Task Should_find_the_newly_created_animal()
    {
        _ = await Page.GotoAsync($"animals/{_id}", new() { WaitUntil = WaitUntilState.NetworkIdle });

        await Expect(Page.Locator("span").Filter(new() { HasText = name })).ToHaveTextAsync(name);
        await Expect(Page.Locator("span").Filter(new() { HasText = kind.ToString() })).ToHaveTextAsync(kind.ToString());
    }
}
