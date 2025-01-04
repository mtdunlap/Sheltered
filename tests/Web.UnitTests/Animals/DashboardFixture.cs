using System;
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
using System.Collections.Generic;

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

        Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
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

        Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
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
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { Guid.NewGuid(), animalModel}
            });

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(p => p.ChildContent)!);

        Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
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
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { Guid.NewGuid(), animalModel}
            });

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

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { id, animalModel}
            });

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>("<div>AnimalPreview</div>");

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches(@$"
                <h1>Animals</h1>
                <div>
                    <a href=""animals/{id}"">
                        <div>AnimalPreview</div>
                    </a>
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
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { Guid.NewGuid(), lucyTheCat },
                { Guid.NewGuid(), jakeTheDog },
                { Guid.NewGuid(), neekoTheCat }
            });

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
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { Guid.NewGuid(), lucyTheCat },
                { Guid.NewGuid(), jakeTheDog },
                { Guid.NewGuid(), neekoTheCat }
            });

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();
        testContext.ComponentFactories.AddStub<AnimalPreview>();

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var pageTitleStub = dashboardPage.FindComponent<Stub<PageTitle>>();
        var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(pageTitle => pageTitle.ChildContent)!);

        Assert.That(pageTitle.Markup, Is.EqualTo("Animals Dashboard"));
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_And_returns_three_animal_model()
    {
        using var testContext = new Bunit.TestContext();

        using var shelteredClient = Substitute.For<IShelteredClient>();

        var lucysId = Guid.NewGuid();
        var lucyTheCat = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var jakesId = Guid.NewGuid();
        var jakeTheDog = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog
        };
        var neekosId = Guid.NewGuid();
        var neekoTheCat = new AnimalModel
        {
            Name = "Neeko",
            Kind = AnimalKind.Cat
        };
        shelteredClient
            .ListAnimalsAsync(Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, AnimalModel>
            {
                { lucysId, lucyTheCat },
                { jakesId, jakeTheDog },
                { neekosId, neekoTheCat }
            });

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<AnimalPreview>("<div>AnimalPreview</div>");

        using var dashboardPage = testContext.RenderComponent<Dashboard>();

        var animalPreviewComponents = dashboardPage.FindComponents<Stub<AnimalPreview>>();

        Assert.That(() =>
        {
            dashboardPage.MarkupMatches($@"
                <h1>Animals</h1>
                <div>
                    <a href=""animals/{lucysId}"">
                        <div>AnimalPreview</div>
                    </a>
                    <a href=""animals/{jakesId}"">
                        <div>AnimalPreview</div>
                    </a>
                    <a href=""animals/{neekosId}"">
                        <div>AnimalPreview</div>
                    </a>
                </div>
            ");
        }, Throws.Nothing);
    }
}
