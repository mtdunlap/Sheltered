using System.Collections.Generic;
using Api.Animals;
using Client.Animals;
using Core.Animals;
using Data.Animals;

namespace Api.UnitTests.Animals;

[TestFixture]
[TestOf(typeof(AnimalMapper))]
[Parallelizable(ParallelScope.All)]
internal sealed class AnimalMapperFixture
{
    private static IEnumerable<TestCaseData> CreateTestCaseSource()
    {
        yield return new TestCaseData(new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat })
            .Returns(new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Cat });
    }

    [Test, TestCaseSource(nameof(CreateTestCaseSource))]
    [TestOf(nameof(AnimalMapper.Create))]
    public AnimalEntity Create__Should_return_an_animal_entity_created_from_an_animal_model(AnimalModel animalModel)
    {
        var animalMapper = new AnimalMapper();
        return animalMapper.Create(animalModel);
    }

    private static IEnumerable<TestCaseData> MapTestCaseSource()
    {
        yield return new TestCaseData(new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Cat })
            .Returns(new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat });
    }

    [Test, TestCaseSource(nameof(MapTestCaseSource))]
    [TestOf(nameof(AnimalMapper.Map))]
    public AnimalModel Map__Should_return_an_animal_model_mapped_from_an_animal_entity(AnimalEntity animalEntity)
    {
        var animalMapper = new AnimalMapper();
        return animalMapper.Map(animalEntity);
    }

    private static IEnumerable<TestCaseData> UpdateTestCaseSource()
    {
        yield return new TestCaseData(new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Dog }, new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat })
            .Returns(new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Cat });
    }

    [Test, TestCaseSource(nameof(UpdateTestCaseSource))]
    [TestOf(nameof(AnimalMapper.Update))]
    public AnimalEntity Update__Should_update_an_animal_entity_from_an_animal_model(AnimalEntity animalEntity, AnimalModel animalModel)
    {
        var animalMapper = new AnimalMapper();
        animalMapper.Update(animalEntity, animalModel);
        return animalEntity;
    }
}
