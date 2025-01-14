using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Client.Animals;
using Core.Animals;

namespace Web.IntegrationTests.Animals;

[TestFixture]
[NonParallelizable]
internal sealed class DashboardPageFixture : BlazorPageFixture
{
    [Test]
    public async Task Should_render_the_title()
    {
        var dashboardPage = new DashboardPage(Page);
        _ = await dashboardPage.GotoAsync();

        Assert.That(async () =>
        {
            await Expect(Page).ToHaveTitleAsync("Animals Dashboard");
        }, Throws.Nothing);
    }

    [Test]
    public async Task Should_navigate_to_the_create_animal_page_When_the_add_a_new_animal_button_is_clicked()
    {
        var dashboardPage = new DashboardPage(Page);
        _ = await dashboardPage.GotoAsync(new() { WaitUntil = WaitUntilState.NetworkIdle });
        await dashboardPage.AddAnimalButton.ClickAsync();

        Assert.That(async () =>
        {
            await Expect(Page).ToHaveURLAsync(NewAnimalPage.Url);
        }, Throws.Nothing);
    }

    [Test]
    public async Task Should_navigate_to_the_animal_detail_page_When_the_animal_preview_link_is_clicked()
    {
        var lucysId = Guid.NewGuid();
        var lucyTheCat = new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat, Sex = AnimalSex.Female };
        MockApi
            .Given(
                Request.Create()
                    .WithPath("/animal")
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new Dictionary<Guid, AnimalModel>()
                    {
                        { lucysId, lucyTheCat }
                    })
            );

        var dashboardPage = new DashboardPage(Page);
        _ = await dashboardPage.GotoAsync(new() { WaitUntil = WaitUntilState.NetworkIdle });
        await dashboardPage.GetAnimalDetailLink(lucyTheCat).ClickAsync();

        Assert.That(async () =>
        {
            await Expect(Page).ToHaveURLAsync(AnimalDetailPage.GetRelativeUrl(lucysId));
        }, Throws.Nothing);
    }
}
