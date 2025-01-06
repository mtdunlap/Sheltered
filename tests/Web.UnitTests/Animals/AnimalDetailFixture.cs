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
internal sealed class AnimalDetailFixture
{
    [Test]
    public void Should_render_the_title_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(pageTitle.Markup, Is.Empty);
        });
    }

    [Test]
    public void Should_not_render_the_update_button_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = newAnimalPage.Find("button");
            }, Throws.TypeOf<ElementNotFoundException>());
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1></h1>
                    <div>
                        <label>Name</label>
                        <span></span>
                        <label>Kind</label>
                        <span></span>
                    </div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_title_When_the_sheltered_client_responds_successfully()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(pageTitle.Markup, Is.EqualTo("Lucy"));
        });
    }

    [Test]
    public void Should_render_the_update_button_When_the_sheltered_client_responds_successfully()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = newAnimalPage.Find("button");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Lucy</h1>
                    <div>
                        <label>Name</label>
                        <span>Lucy</span>
                        <label>Kind</label>
                        <span>Cat</span>
                    </div>
                    <button id=""updateButton"" class=""btn btn-primary"">Update Animal</button>
                    <button id=""deleteButton"" class=""btn btn-primary"">Delete Animal</button>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_navigate_to_the_update_animal_page_When_clicking_the_update_animal_button()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        var button = newAnimalPage.Find("button");
        button.Click();

        var navigationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            var expectedRelativeUrl = $"animals/update/{id}";
            var expected = navigationManager.BaseUri + expectedRelativeUrl;
            Assert.That(navigationManager.Uri, Is.EqualTo(expected));
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_clicking_the_delete_animal_button_And_the_sheltered_client_throws_an_exception()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        shelteredClient
            .DeleteAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var animalDetailPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        var deleteButton = animalDetailPage.Find("#deleteButton");
        deleteButton.Click();

        using var pageTitleStub = animalDetailPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var navigationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Lucy"));
            Assert.That(() =>
            {
                animalDetailPage.MarkupMatches(@"
                    <h1>Lucy</h1>
                    <div>
                        <label>Name</label>
                        <span>Lucy</span>
                        <label>Kind</label>
                        <span>Cat</span>
                    </div>
                    <button id=""updateButton"" class=""btn btn-primary"">Update Animal</button>
                    <button id=""deleteButton"" class=""btn btn-primary"">Delete Animal</button>
                    <div>An unknown error occurred, please try again momentarily.</div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_clicking_the_delete_animal_button_And_the_sheltered_client_cannot_find_the_animal()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        shelteredClient
            .DeleteAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(false);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var animalDetailPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        var deleteButton = animalDetailPage.Find("#deleteButton");
        deleteButton.Click();

        using var pageTitleStub = animalDetailPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var navigationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Lucy"));
            Assert.That(() =>
            {
                animalDetailPage.MarkupMatches(@"
                    <h1>Lucy</h1>
                    <div>
                        <label>Name</label>
                        <span>Lucy</span>
                        <label>Kind</label>
                        <span>Cat</span>
                    </div>
                    <button id=""updateButton"" class=""btn btn-primary"">Update Animal</button>
                    <button id=""deleteButton"" class=""btn btn-primary"">Delete Animal</button>
                    <div>The animal could not be deleted as it does not exist.</div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_navigate_to_the_animals_dashboard_When_clicking_the_delete_animal_button_And_the_sheltered_client_successfully_deletes_the_animal()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        shelteredClient
            .DeleteAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(true);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var animalDetailPage = testContext.RenderComponent<AnimalDetail>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        var deleteButton = animalDetailPage.Find("#deleteButton");
        deleteButton.Click();

        using var pageTitleStub = animalDetailPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var navigationManager = testContext.Services.GetRequiredService<FakeNavigationManager>();

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            const string expectedRelativeUrl = "animals";
            var expected = navigationManager.BaseUri + expectedRelativeUrl;
            Assert.That(navigationManager.Uri, Is.EqualTo(expected));
            Assert.That(pageTitle.Markup, Is.EqualTo("Lucy"));
        });
    }
}
