using Microsoft.AspNetCore.Http;
using NSubstitute;
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
    [Test]
    [TestOf(nameof(AnimalMapper.Create))]
    public void Create__Should_return_an_animal_entity_created_from_an_animal_model()
    {
        var animalImageMapper = Substitute.For<IAnimalImageMapper>();
        var animalMapper = new AnimalMapper(animalImageMapper);

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female,
            Images = []
        };

        var actual = animalMapper.Create(animalModel);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Name, Is.EqualTo("Lucy"));
            Assert.That(actual.Kind, Is.EqualTo(AnimalKind.Cat));
            Assert.That(actual.Sex, Is.EqualTo(AnimalSex.Female));
            Assert.That(actual.Images, Is.Empty);
        });
    }

    [Test]
    [TestOf(nameof(AnimalMapper.Map))]
    public void Map__Should_return_an_animal_model_mapped_from_an_animal_entity_When_the_animal_has_no_images()
    {
        var animalImageMapper = Substitute.For<IAnimalImageMapper>();
        var httpContext = Substitute.For<HttpContext>();

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female,
            Images = []
        };

        var animalMapper = new AnimalMapper(animalImageMapper);
        var actual = animalMapper.Map(animalEntity, httpContext);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Name, Is.EqualTo("Lucy"));
            Assert.That(actual.Kind, Is.EqualTo(AnimalKind.Cat));
            Assert.That(actual.Sex, Is.EqualTo(AnimalSex.Female));
            Assert.That(actual.Images, Is.Empty);
        });
    }

    [Test]
    [TestOf(nameof(AnimalMapper.Map))]
    public void Map__Should_return_an_animal_model_mapped_from_an_animal_entity_When_the_animal_has_exactly_one_image()
    {
        var animalImageMapper = Substitute.For<IAnimalImageMapper>();
        var httpContext = Substitute.For<HttpContext>();
        var animalImageEntity = new AnimalImageEntity
        {
            Location = "http://localhost/",
            ContentType = "image/png",
            Height = 900,
            Width = 1600,
            FileSize = 100_000
        };
        var animalImageModel = new AnimalImageModel
        {
            Location = "http://localhost/",
            ContentType = "image/png",
            Height = 900,
            Width = 1600,
            FileSize = 100_000
        };
        animalImageMapper
            .Map(animalImageEntity, httpContext)
            .Returns(animalImageModel);

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female,
            Images = [
                animalImageEntity
            ]
        };

        var animalMapper = new AnimalMapper(animalImageMapper);
        var actual = animalMapper.Map(animalEntity, httpContext);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Name, Is.EqualTo("Lucy"));
            Assert.That(actual.Kind, Is.EqualTo(AnimalKind.Cat));
            Assert.That(actual.Sex, Is.EqualTo(AnimalSex.Female));
            Assert.That(actual.Images, Has.Exactly(1).Items);
            Assert.That(actual.Images, Has.Exactly(1).SameAs(animalImageModel));
        });
    }

    [Test]
    [TestOf(nameof(AnimalMapper.Update))]
    public void Update__Should_update_an_animal_entity_from_an_animal_model()
    {
        var animalImageMapper = Substitute.For<IAnimalImageMapper>();
        var animalMapper = new AnimalMapper(animalImageMapper);

        var animalEntity = new AnimalEntity
        {
            Name = "Jake",
            Kind = AnimalKind.Dog,
            Sex = AnimalSex.Male,
            Images = []
        };
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female,
            Images = []
        };

        animalMapper.Update(animalEntity, animalModel);

        Assert.Multiple(() =>
        {
            Assert.That(animalEntity.Name, Is.EqualTo("Lucy"));
            Assert.That(animalEntity.Kind, Is.EqualTo(AnimalKind.Cat));
            Assert.That(animalEntity.Sex, Is.EqualTo(AnimalSex.Female));
            Assert.That(animalEntity.Images, Is.Empty);
        });
    }
}
