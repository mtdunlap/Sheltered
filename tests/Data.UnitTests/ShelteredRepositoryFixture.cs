using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Animals;
using Data.Animals;
using System.Collections.Generic;

namespace Data.UnitTests;

[TestFixture]
[TestFixtureSource(typeof(ShelteredRepositoryFixtureSource))]
internal sealed class ShelteredRepositoryFixture(IDbContextOptionsFactory dbContextOptionsFactory,
    CancellationTokenSource cancellationTokenSource)
        : RepositoryFixture<ShelteredContext>(dbContextOptionsFactory, cancellationTokenSource)
{
    protected override ShelteredContext CreateContext(DbContextOptions dbContextOptions) => new(dbContextOptions);

    [Test]
    public async Task AddAnimalAsync__Should_add_the_animal_entity_to_the_sheltered_context()
    {
        var repository = new ShelteredRepository(Context);

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await repository.AddAnimalAsync(animalEntity, CancellationToken);
        await repository.SaveChangesAsync(CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(Context.Animals, Has.Exactly(1).Items);
            Assert.That(Context.Animals, Has.Exactly(1).EqualTo(animalEntity));
        });
    }

    [Test]
    public async Task AnimalExistsByIdAsync__Should_return_false_When_the_sheltered_context_does_not_contain_an_animal_entity_with_the_provided_id()
    {
        var repository = new ShelteredRepository(Context);

        var id = Guid.NewGuid();
        var actual = await repository.AnimalExistsByIdAsync(id, CancellationToken);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task AnimalExistsByIdAsync__Should_return_true_When_the_sheltered_context_contains_an_animal_entity_with_the_provided_id_exists_in_the_sheltered_context()
    {
        var repository = new ShelteredRepository(Context);

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await Context.AddAsync(animalEntity, CancellationToken);
        await Context.SaveChangesAsync(CancellationToken);

        var actual = await repository.AnimalExistsByIdAsync(animalEntity.Id, CancellationToken);

        Assert.That(actual, Is.True);
    }

    [Test]
    public async Task GetAnimalByIdAsync__Should_return_null_When_an_animal_entity_with_the_provided_id_does_not_exist_in_the_sheltered_context()
    {
        var id = Guid.NewGuid();
        var repository = new ShelteredRepository(Context);

        var actual = await repository.GetAnimalByIdAsync(id, CancellationToken);

        Assert.That(actual, Is.Null);
    }

    [Test]
    public async Task GetAnimalByIdAsync__Should_return_an_animal_entity_equal_to_the_added_instance_When_an_animal_entity_with_the_provided_id_exists_in_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await Context.AddAsync(animalEntity, CancellationToken);
        await Context.SaveChangesAsync(CancellationToken);

        var repository = new ShelteredRepository(Context);

        var actual = await repository.GetAnimalByIdAsync(animalEntity.Id, CancellationToken);

        Assert.That(actual, Is.EqualTo(animalEntity));
    }

    [Test]
    public async Task ListAnimalsAsync__Should_return_the_entire_list_of_animal_entities()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        var animalEntities = new List<AnimalEntity>
        {
            animalEntity
        };
        await Context.AddRangeAsync(animalEntities, CancellationToken);
        await Context.SaveChangesAsync(CancellationToken);

        var repository = new ShelteredRepository(Context);

        var actual = await repository.ListAnimalsAsync(CancellationToken);

        Assert.That(actual, Is.EquivalentTo(animalEntities));
    }

    [Test]
    public void RemoveAnimal__Should_throw_a_db_update_concurrency_exception_When_the_animal_entity_does_not_exist_in_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        var repository = new ShelteredRepository(Context);

        repository.RemoveAnimal(animalEntity);

        Assert.That(async () =>
        {
            await repository.SaveChangesAsync(CancellationToken);
        }, Throws.TypeOf<DbUpdateConcurrencyException>());
    }

    [Test]
    public async Task RemoveAnimal__Should_remove_the_animal_entity_When_the_animal_entity_exists_in_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await Context.AddAsync(animalEntity, CancellationToken);
        await Context.SaveChangesAsync(CancellationToken);

        var repository = new ShelteredRepository(Context);

        repository.RemoveAnimal(animalEntity);
        await repository.SaveChangesAsync(CancellationToken);

        Assert.That(Context.Animals, Is.Empty);
    }

    [Test]
    public async Task SaveChangesAsync__Should_save_any_changes_to_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await Context.AddAsync(animalEntity, CancellationToken);

        var repository = new ShelteredRepository(Context);

        Assert.Multiple(async () =>
        {
            Assert.That(Context.Animals, Is.Empty);

            await repository.SaveChangesAsync(CancellationToken);

            Assert.That(Context.Animals, Has.Exactly(1).Items);
            Assert.That(Context.Animals, Has.Exactly(1).EqualTo(animalEntity));
        });
    }

    [Test]
    public async Task UpdateAnimal__Should_add_the_animal_entity_to_the_sheltered_context_When_the_animal_entity_is_not_already_in_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };

        var repository = new ShelteredRepository(Context);

        repository.UpdateAnimal(animalEntity);
        await repository.SaveChangesAsync(CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(Context.Animals, Has.Exactly(1).Items);
            Assert.That(Context.Animals, Has.Exactly(1).EqualTo(animalEntity));
        });
    }

    [Test]
    public async Task UpdateAnimal__Should_update_the_animal_entity_When_the_animal_entity_exists_in_the_sheltered_context()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        await Context.AddAsync(animalEntity, CancellationToken);
        await Context.SaveChangesAsync(CancellationToken);

        var repository = new ShelteredRepository(Context);

        animalEntity.Name = "Jake";
        animalEntity.Kind = AnimalKind.Dog;

        repository.UpdateAnimal(animalEntity);
        await repository.SaveChangesAsync(CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(Context.Animals, Has.Exactly(1).Items);
            Assert.That(Context.Animals, Has.Exactly(1).EqualTo(animalEntity));
        });
    }
}
