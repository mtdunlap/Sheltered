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
                        ");
            }, Throws.Nothing);
        });
    }
}
