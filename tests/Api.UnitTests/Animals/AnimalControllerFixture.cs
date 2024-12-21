using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using Api.Animals;
using Client.Animals;
using Core.Animals;
using Data;
using Data.Animals;

namespace Api.UnitTests.Animals;

[TestFixture]
[TestOf(typeof(AnimalController))]
[Parallelizable(ParallelScope.All)]
internal sealed class AnimalControllerFixture
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();

    [Test]
    [TestOf(nameof(AnimalController.Head))]
    public async Task Head__Should_return_a_not_found_result_When_an_animal_does_not_exist_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .AnimalExistsByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .Returns(false);

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Head(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NotFoundResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).AnimalExistsByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Head))]
    public async Task Head__Should_return_a_no_content_result_When_an_animal_exists_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .AnimalExistsByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .Returns(true);

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Head(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NoContentResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).AnimalExistsByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Get))]
    public async Task Get__Should_return_a_not_found_result_When_an_animal_does_not_exist_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .ReturnsNull();

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Get(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NotFoundResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Get))]
    public async Task Get__Should_return_an_ok_object_result_with_the_animal_model_When_an_animal_exists_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .Returns(animalEntity);

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var animalMapper = Substitute.For<IAnimalMapper>();
        animalMapper
            .Map(Arg.Is(animalEntity))
            .Returns(animalModel);

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Get(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<OkObjectResult>());
            Assert.That((actual as OkObjectResult)?.Value, Is.Not.Null.And.SameAs(animalModel));
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = animalMapper.Received(Quantity.Exactly(1)).Map(Arg.Is(animalEntity));
            }, Throws.Nothing);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Post))]
    public void Post__Should_thrown_an_invalid_operation_exception_When_the_created_animal_is_not_retrieved()
    {
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var animalMapper = Substitute.For<IAnimalMapper>();
        animalMapper
            .Create(Arg.Is(animalModel))
            .Returns(animalEntity);

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .AddAnimalAsync(Arg.Is(animalEntity), Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        shelteredRepository
            .SaveChangesAsync(Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        var createdEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(animalEntity.Id), Arg.Is(cancellationToken))
            .ReturnsNull();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await animalController.Post(animalModel, cancellationToken);
            }, Throws.InvalidOperationException);
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(3).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).AddAnimalAsync(Arg.Is(animalEntity), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).SaveChangesAsync(Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(animalEntity.Id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = animalMapper.Received(Quantity.Exactly(1)).Create(Arg.Is(animalModel));
            }, Throws.Nothing);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Post))]
    public async Task Post__Should_return_a_created_at_action_result_When_the_animal_is_successfully_created()
    {
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var animalMapper = Substitute.For<IAnimalMapper>();
        animalMapper
            .Create(Arg.Is(animalModel))
            .Returns(animalEntity);

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .AddAnimalAsync(Arg.Is(animalEntity), Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        shelteredRepository
            .SaveChangesAsync(Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        var createdEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(animalEntity.Id), Arg.Is(cancellationToken))
            .Returns(animalEntity);

        var createdModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        animalMapper
            .Map(Arg.Is(createdEntity))
            .Returns(createdModel);

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Post(animalModel, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<CreatedAtActionResult>());
            Assert.That((actual as CreatedAtActionResult)?.Value, Is.Not.Null.And.SameAs(createdModel));
            Assert.That((actual as CreatedAtActionResult)?.ActionName, Is.Not.Null.And.EqualTo(nameof(AnimalController.Get)));
            Assert.That((actual as CreatedAtActionResult)?.RouteValues, Is.Not.Null.And.ContainKey("id"));
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(3).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).AddAnimalAsync(Arg.Is(animalEntity), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).SaveChangesAsync(Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(animalEntity.Id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(2).Items);
            Assert.That(() =>
            {
                _ = animalMapper.Received(Quantity.Exactly(1)).Create(Arg.Is(animalModel));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = animalMapper.Received(Quantity.Exactly(1)).Map(Arg.Is(createdEntity));
            }, Throws.Nothing);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Put))]
    public async Task Put__Should_return_a_not_found_result_When_an_animal_does_not_exist_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var cancellationToken = CancellationTokenSource.Token;

        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .ReturnsNull();

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Put(id, animalModel, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NotFoundResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Put))]
    public async Task Put__Should_return_a_no_content_result_When_an_animal_with_the_provided_id_is_successfully_updated()
    {
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Dog
        };
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .Returns(animalEntity);

        shelteredRepository
            .SaveChangesAsync(Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Put(id, animalModel, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NoContentResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(3).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                shelteredRepository.Received(Quantity.Exactly(1)).UpdateAnimal(Arg.Is(animalEntity));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).SaveChangesAsync(Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                animalMapper.Received(Quantity.Exactly(1)).Update(Arg.Is(animalEntity), Arg.Is(animalModel));
            }, Throws.Nothing);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Delete))]
    public async Task Delete__Should_return_a_not_found_result_When_an_animal_does_not_exist_with_the_provided_id()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .ReturnsNull();

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Delete(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NotFoundResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(1).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }

    [Test]
    [TestOf(nameof(AnimalController.Delete))]
    public async Task Delete__Should_return_a_no_content_result_When_an_animal_with_the_provided_id_is_successfully_deleted()
    {
        var id = Guid.NewGuid();
        var cancellationToken = CancellationTokenSource.Token;

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        var shelteredRepository = Substitute.For<IShelteredRepository>();
        shelteredRepository
            .GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken))
            .Returns(animalEntity);

        shelteredRepository
            .SaveChangesAsync(Arg.Is(cancellationToken))
            .Returns(Task.CompletedTask);

        var animalMapper = Substitute.For<IAnimalMapper>();

        var animalController = new AnimalController(shelteredRepository, animalMapper);

        var actual = await animalController.Delete(id, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.TypeOf<NoContentResult>());
            Assert.That(shelteredRepository.ReceivedCalls(), Has.Exactly(3).Items);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).GetAnimalByIdAsync(Arg.Is(id), Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                shelteredRepository.Received(Quantity.Exactly(1)).RemoveAnimal(Arg.Is(animalEntity));
            }, Throws.Nothing);
            Assert.That(() =>
            {
                _ = shelteredRepository.Received(Quantity.Exactly(1)).SaveChangesAsync(Arg.Is(cancellationToken));
            }, Throws.Nothing);
            Assert.That(animalMapper.ReceivedCalls(), Has.Exactly(0).Items);
        });
    }
}
