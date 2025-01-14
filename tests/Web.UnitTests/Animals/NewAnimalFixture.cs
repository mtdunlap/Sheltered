using System;
using System.Net.Http;
using System.Threading;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using Client;
using Client.Animals;
using Core.Animals;
using Web.Animals;

namespace Web.UnitTests.Animals;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal sealed class NewAnimalFixture
{
    [Test]
    public void Should_render_the_title_When_the_page_is_first_loaded()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<NewAnimal>();

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        Assert.That(pageTitle.Markup, Is.EqualTo("New Animal"));
    }

    [Test]
    public void Should_render_the_default_templated_html_When_the_page_is_first_loaded()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        testContext.Services.AddSingleton(shelteredClient);

        using var newAnimalPage = testContext.RenderComponent<NewAnimal>();

        Assert.That(() =>
        {
            newAnimalPage.MarkupMatches(@"
                <h1>Add a New Animal</h1>
                <div>
                <label for=""name"">Name</label>
                <input id=""name"" name=""Name"" value=""""  >
                <label for=""kind"">Kind</label>
                <select id=""kind"" name=""Kind"" value=""Unspecified""  >
                    <option value=""Unspecified"" selected="""">Unspecified</option>
                    <option value=""Dog"">Dog</option>
                    <option value=""Cat"">Cat</option>
                </select>
                <label for=""sex"">Sex</label>
                <select id=""sex"" name=""Sex"" value=""Unknown""  >
                    <option value=""Unknown"" selected="""">Unknown</option>
                    <option value=""Male"">Male</option>
                    <option value=""Female"">Female</option>
                </select>
                <button class=""btn btn-primary"" >Submit</button>
                <div hidden=""""></div>
                </div>
            ");
        }, Throws.Nothing);
    }


    [Test]
    public void Should_show_an_error_message_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };

        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .CreateAnimalAsync(Arg.Is(animalModel), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        using var newAnimalPage = testContext.RenderComponent<NewAnimal>();

        var naviagationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        var input = newAnimalPage.Find("input");
        input.Change("Lucy");
        var selectKind = newAnimalPage.Find("#kind");
        selectKind.Change("Cat");
        var selectSex = newAnimalPage.Find("#sex");
        selectSex.Change("Female");

        var button = newAnimalPage.Find("button");
        button.Click();

        Assert.Multiple(() =>
        {
            Assert.That(() =>
            {
                return shelteredClient
                    .Received(Quantity.Exactly(1))
                    .CreateAnimalAsync(
                        Arg.Is<AnimalModel>(animalModel =>
                            string.Equals(animalModel.Name, "Lucy", StringComparison.InvariantCulture)
                            && animalModel.Kind == AnimalKind.Cat && animalModel.Sex == AnimalSex.Female),
                        Arg.Is(CancellationToken.None)
                    );
            }, Throws.Nothing);
            Assert.That(naviagationManager.History, Is.Empty);
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Add a New Animal</h1>
                    <div>
                        <label for=""name"">Name</label>
                        <input id=""name"" name=""Name"" value=""Lucy""/>
                        <label for=""kind"">Kind</label>
                        <select id=""kind"" name=""Kind"" value=""Cat"">
                            <option value=""Unspecified"">Unspecified</option>
                            <option value=""Dog"">Dog</option>
                            <option value=""Cat"" selected="""">Cat</option>
                        </select>
                        <label for=""sex"">Sex</label>
                        <select id=""sex"" name=""Sex"" value=""Female""  >
                            <option value=""Unknown"" >Unknown</option>
                            <option value=""Male"">Male</option>
                            <option value=""Female"" selected="""">Female</option>
                        </select>
                        <button class=""btn btn-primary"">Submit</button>
                        <div>An error occurred, please try again momentarily.</div>
                    </div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_submit_the_new_animal_and_navigate_to_the_new_animals_details_page_When_the_sheltered_client_responds_successfully()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };

        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .CreateAnimalAsync(Arg.Is(animalModel), Arg.Is(CancellationToken.None))
            .Returns((animalModel, id));

        testContext.Services.AddSingleton(shelteredClient);

        using var newAnimalPage = testContext.RenderComponent<NewAnimal>();

        var naviagationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        var input = newAnimalPage.Find("input");
        input.Change("Lucy");
        var selectKind = newAnimalPage.Find("#kind");
        selectKind.Change("Cat");
        var selectSex = newAnimalPage.Find("#sex");
        selectSex.Change("Female");

        var button = newAnimalPage.Find("button");
        button.Click();

        Assert.Multiple(() =>
        {
            Assert.That(() =>
            {
                return shelteredClient
                    .Received(Quantity.Exactly(1))
                    .CreateAnimalAsync(
                        Arg.Is<AnimalModel>(animalModel =>
                            string.Equals(animalModel.Name, "Lucy", StringComparison.InvariantCulture)
                            && animalModel.Kind == AnimalKind.Cat && animalModel.Sex == AnimalSex.Female),
                        Arg.Is(CancellationToken.None)
                    );
            }, Throws.Nothing);
            var expectedRelativeUrl = $"animals/{id}";
            var expected = $"{naviagationManager.BaseUri}{expectedRelativeUrl}";
            Assert.That(naviagationManager.Uri, Is.EqualTo(expected));

            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Add a New Animal</h1>
                    <div>
                    <label for=""name"">Name</label>
                    <input id=""name"" name=""Name"" value=""Lucy""  >
                    <label for=""kind"">Kind</label>
                    <select id=""kind"" name=""Kind"" value=""Cat""  >
                        <option value=""Unspecified"">Unspecified</option>
                        <option value=""Dog"">Dog</option>
                        <option value=""Cat"" selected="""">Cat</option>
                    </select>
                    <label for=""sex"">Sex</label>
                    <select id=""sex"" name=""Sex"" value=""Female""  >
                        <option value=""Unknown"">Unknown</option>
                        <option value=""Male"">Male</option>
                        <option value=""Female"" selected="""">Female</option>
                    </select>
                    <button class=""btn btn-primary"" >Submit</button>
                    <div hidden=""""></div>
                    </div>
                ");
            }, Throws.Nothing);
        });
    }
}
