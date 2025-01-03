using System.Collections.Generic;
using Bunit;
using Client.Animals;
using Core.Animals;
using Web.Animals;

namespace Web.UnitTests.Animals;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal sealed class AnimalPreviewFixture
{
    private static IEnumerable<TestCaseData> AnimalModelAndExpectedHtmlSource()
    {
        yield return new TestCaseData(new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        },
        @"
            <div>
                <label>Name</label>
                <span>Lucy</span>
                <label>Kind</label>
                <span>Cat</span>
            </div>
        "
        );
        yield return new TestCaseData(new AnimalModel
        {
            Name = "Neeko",
            Kind = AnimalKind.Cat
        },
        @"
            <div>
                <label>Name</label>
                <span>Neeko</span>
                <label>Kind</label>
                <span>Cat</span>
            </div>
        "
        );
        yield return new TestCaseData(new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog
        },
        @"
            <div>
                <label>Name</label>
                <span>Jake</span>
                <label>Kind</label>
                <span>Dog</span>
            </div>
        "
        );
    }

    [Test]
    [TestCaseSource(nameof(AnimalModelAndExpectedHtmlSource))]
    public void Should_render_the_templated_html(AnimalModel animalModel, string expectedHtml)
    {
        using var testContext = new Bunit.TestContext();

        using var animalPreviewComponent = testContext.RenderComponent<AnimalPreview>(parameters =>
        {
            parameters.Add(parameter => parameter.Animal, animalModel);
        });

        Assert.That(() =>
        {
            animalPreviewComponent.MarkupMatches(expectedHtml);
        }, Throws.Nothing);
    }
}
