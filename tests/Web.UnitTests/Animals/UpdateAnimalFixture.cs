using System;
using System.Net;
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
internal sealed class UpdateAnimalFixture
{
    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_throws_an_exception_on_page_load()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<Exception>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .DidNotReceive()
                    .UpdateAnimalByIdAsync(Arg.Any<Guid>(), Arg.Any<AnimalModel>(), Arg.Any<CancellationToken>());
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>An unknown error occurred, please try again momentarily.</div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_throws_an_http_request_exception_on_page_load()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .DidNotReceive()
                    .UpdateAnimalByIdAsync(Arg.Any<Guid>(), Arg.Any<AnimalModel>(), Arg.Any<CancellationToken>());
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>An unknown error occurred, please try again momentarily.</div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_throws_an_http_request_exception_on_page_load_With_a_status_code_of_404_not_found()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .ThrowsAsync(new HttpRequestException(string.Empty, null, HttpStatusCode.NotFound));

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .DidNotReceive()
                    .UpdateAnimalByIdAsync(Arg.Any<Guid>(), Arg.Any<AnimalModel>(), Arg.Any<CancellationToken>());
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>The requested animal was not found.</div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_on_page_load()
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
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(animalModel);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .DidNotReceive()
                    .UpdateAnimalByIdAsync(Arg.Any<Guid>(), Arg.Any<AnimalModel>(), Arg.Any<CancellationToken>());
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>
                        <label>Current Name</label>
                        <span>Lucy</span>
                        <label>Current Kind</label>
                        <span>Cat</span>
                        <label>Current Sex</label>
                        <span>Female</span>
                    </div>
                    <div>
                        <label for=""name"">New Name</label>
                        <input id=""name"" name=""Name"" value=""""/>
                        <label for=""kind"">New Kind</label>
                        <select id=""kind"" name=""Kind"" value=""Unspecified"">
                            <option value=""Unspecified"" selected="""">Unspecified</option>
                            <option value=""Dog"">Dog</option>
                            <option value=""Cat"">Cat</option>
                        </select>
                        <label for=""sex"">New Sex</label>
                        <select id=""sex"" name=""Sex"" value=""Unknown""  >
                            <option value=""Unknown"" selected="""">Unknown</option>
                            <option value=""Male"">Male</option>
                            <option value=""Female"">Female</option>
                        </select>
                        <button class=""btn btn-primary"">Submit</button>
                    </div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_on_page_load_And_the_sheltered_client_throws_an_http_request_exception_when_updating_the_animal()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var initialAnimalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(initialAnimalModel);

        var updateAnimalModel = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog,
            Sex = AnimalSex.Male
        };
        shelteredClient
            .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None))
            .ThrowsAsync<HttpRequestException>();

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var input = newAnimalPage.Find("input");
        input.Change("Jake");
        var selectKind = newAnimalPage.Find("#kind");
        selectKind.Change("Dog");
        var selectSex = newAnimalPage.Find("#sex");
        selectSex.Change("Male");

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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>
                    <label>Current Name</label>
                    <span>Lucy</span>
                    <label>Current Kind</label>
                    <span>Cat</span>
                    <label>Current Sex</label>
                    <span>Female</span>
                    </div>
                    <div>
                    <label for=""name"">New Name</label>
                    <input id=""name"" name=""Name"" value=""Jake""  >
                    <label for=""kind"">New Kind</label>
                    <select id=""kind"" name=""Kind"" value=""Dog""  >
                        <option value=""Unspecified"">Unspecified</option>
                        <option value=""Dog"" selected="""">Dog</option>
                        <option value=""Cat"">Cat</option>
                    </select>
                    <label for=""sex"">New Sex</label>
                    <select id=""sex"" name=""Sex"" value=""Male""  >
                        <option value=""Unknown"">Unknown</option>
                        <option value=""Male"" selected="""">Male</option>
                        <option value=""Female"">Female</option>
                    </select>
                    <button class=""btn btn-primary"" >Submit</button>
                    <div>An unknown error occurred, please try again momentarily.</div>
                    </div>
                ");
            Assert.That(() =>
            {

            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_render_the_templated_html_When_the_sheltered_client_responds_successfully_on_page_load_And_the_sheltered_client_responsds_successfully_with_false_when_updating_the_animal()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var initialAnimalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(initialAnimalModel);

        var updateAnimalModel = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog,
            Sex = AnimalSex.Male
        };
        shelteredClient
            .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None))
            .Returns(false);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var input = newAnimalPage.Find("input");
        input.Change("Jake");
        var selectKind = newAnimalPage.Find("#kind");
        selectKind.Change("Dog");
        var selectSex = newAnimalPage.Find("#sex");
        selectSex.Change("Male");

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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            Assert.That(navigationManager.History, Is.Empty);
            Assert.That(pageTitle.Markup, Is.EqualTo("Update"));
            Assert.That(() =>
            {
                newAnimalPage.MarkupMatches(@"
                    <h1>Update Animal</h1>
                    <div>
                        <label>Current Name</label>
                        <span>Lucy</span>
                        <label>Current Kind</label>
                        <span>Cat</span>
                        <label>Current Sex</label>
                        <span>Female</span>
                    </div>
                    <div>
                        <label for=""name"">New Name</label>
                        <input id=""name"" name=""Name"" value=""Jake""/>
                        <label for=""kind"">New Kind</label>
                        <select id=""kind"" name=""Kind"" value=""Dog"">
                            <option value=""Unspecified"">Unspecified</option>
                            <option value=""Dog"" selected="""">Dog</option>
                            <option value=""Cat"">Cat</option>
                        </select>
                        <label for=""sex"">New Sex</label>
                        <select id=""sex"" name=""Sex"" value=""Male""  >
                            <option value=""Unknown"">Unknown</option>
                            <option value=""Male"" selected="""">Male</option>
                            <option value=""Female"">Female</option>
                        </select>
                            <button class=""btn btn-primary"">Submit</button>
                            <div>The animal could not be updated as it does not exist.</div>
                        </div>
                ");
            }, Throws.Nothing);
        });
    }

    [Test]
    public void Should_navigate_to_the_animal_detail_page_When_the_sheltered_client_responds_successfully_on_page_load_And_the_sheltered_client_responsds_successfully_with_true_when_updating_the_animal()
    {
        using var testContext = new Bunit.TestContext();

        var id = Guid.NewGuid();
        var initialAnimalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var shelteredClient = Substitute.For<IShelteredClient>();
        shelteredClient
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(CancellationToken.None))
            .Returns(initialAnimalModel);

        var updateAnimalModel = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog,
            Sex = AnimalSex.Male
        };
        shelteredClient
            .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None))
            .Returns(true);

        testContext.Services.AddSingleton(shelteredClient);

        testContext.ComponentFactories.AddStub<PageTitle>();

        using var newAnimalPage = testContext.RenderComponent<UpdateAnimal>(parameters =>
        {
            parameters.Add(parameter => parameter.Id, id);
        });

        using var pageTitleStub = newAnimalPage.FindComponent<Stub<PageTitle>>();
        using var pageTitle = testContext.Render(pageTitleStub.Instance.Parameters.Get(title => title.ChildContent)!);

        var input = newAnimalPage.Find("input");
        input.Change("Jake");
        var selectKind = newAnimalPage.Find("#kind");
        selectKind.Change("Dog");
        var selectSex = newAnimalPage.Find("#sex");
        selectSex.Change("Male");

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
            Assert.That(async () =>
            {
                _ = await shelteredClient
                    .Received(Quantity.Exactly(1))
                    .UpdateAnimalByIdAsync(Arg.Is(id), Arg.Is(updateAnimalModel), Arg.Is(CancellationToken.None));
            }, Throws.Nothing);
            var expectedRelativeUrl = $"animals/{id}";
            var expected = navigationManager.BaseUri + expectedRelativeUrl;
            Assert.That(navigationManager.Uri, Is.EqualTo(expected));
        });
    }
}
