using System.Net.Http;
using System.Threading;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Client;
using Client.Animals;
using Core.Animals;
using Web.Animals;

namespace Web.UnitTests.Animals;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public sealed partial class DashboardFixture
{
    [Test]
    public void Should_not_render_any_animal_previews_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(animalPreviewComponents, Is.Empty);
    }

    [Test]
    public void Should_render_the_title_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches(@"
                <h1>Animals</h1>
                <div></div>
            ");
        }, Throws.Nothing);
    }

    [Test]
    public void Should_not_render_any_animal_previews_When_the_sheltered_client_responds_successfully_And_returns_no_animal_models()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(animalPreviewComponents, Is.Empty);
    }

    [Test]
    public void Should_render_the_title_When_the_sheltered_client_responds_successfully_And_returns_no_animal_models()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_And_returns_no_animal_models()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches(@"
                <h1>Animals</h1>
                <div></div>
            ");
        }, Throws.Nothing);
    }

    [Test]
    public void Should_render_the_title_When_the_sheltered_client_responds_successfully_And_returns_one_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([animalModel]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
        });
    }

    [Test]
    public void Should_render_one_animal_preview_When_the_sheltered_client_responds_successfully_And_returns_one_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([animalModel]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(animalPreviewComponents, Has.Exactly(1).Items);
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_And_returns_one_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([animalModel]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>("<div>AnimalPreview</div>");

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches(@"
                <h1>Animals</h1>
                <div>
                    <div>AnimalPreview</div>
                </div>
            ");
        }, Throws.Nothing);
    }

    [Test]
    public void Should_render_three_animal_preview_When_the_sheltered_client_responds_successfully_And_returns_three_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var lucyTheCat = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };

        var jakeTheDog = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog
        };

        var neekoTheCat = new AnimalModel
        {
            Name = "Neeko",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([lucyTheCat, jakeTheDog, neekoTheCat]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(animalPreviewComponents, Has.Exactly(3).Items);
    }

    [Test]
    public void Should_render_the_title_When_the_sheltered_client_responds_successfully_And_returns_three_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([animalModel]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_And_returns_three_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var lucyTheCat = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };

        var jakeTheDog = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog
        };

        var neekoTheCat = new AnimalModel
        {
            Name = "Neeko",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns([lucyTheCat, jakeTheDog, neekoTheCat]);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>("<div>AnimalPreview</div>");

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches(@"
                <h1>Animals</h1>
                <div>
                    <div>AnimalPreview</div>
                    <div>AnimalPreview</div>
                    <div>AnimalPreview</div>
                </div>
            ");
        }, Throws.Nothing);
    }
}
