using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Client.Animals;
using Core.Animals;
using Data;
using Data.Animals;

namespace Api.IntegrationTests.Animals;

[TestFixture]
[NonParallelizable]
internal sealed class AnimalEndpointsFixture : ApiFixture
{
    [Test]
    public async Task AnimalExistsByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var id = Guid.NewGuid();

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task AnimalExistsByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.True);
    }

    [Test]
    public async Task CreateAnimalAsync__Should_insert_a_new_animal_into_the_sheltered_database()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };

        var (created, id) = await shelteredClient.CreateAnimalAsync(animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = GetDbContext<ShelteredContext>();
            Assert.That(shelteredContext.Animals, Has.Exactly(1).Items);
            Assert.That(shelteredContext.Animals, Has.Exactly(1).Matches<AnimalEntity>(animalEntity =>
            {
                return animalEntity.Id == id
                    && string.Equals(animalEntity.Name, animalModel.Name, StringComparison.InvariantCulture)
                    && animalEntity.Kind == animalModel.Kind;
            }));
        });
    }

    [Test]
    public async Task DeleteAnimalByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var id = Guid.NewGuid();

        var actual = await shelteredClient.DeleteAnimalByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task DeleteAnimalByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var actual = await shelteredClient.DeleteAnimalByIdAsync(id, cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(async () =>
            {
                await using var shelteredContext = GetDbContext<ShelteredContext>();
                return await shelteredContext.FindAsync<AnimalEntity>([id], cancellationTokenSource.Token);
            }, Is.Null);
        });
    }

    [Test]
    public void GetAnimalByIdAsync__Should_throw_an_http_request_exception_When_no_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var id = Guid.NewGuid();

        const string expectedMessage = "statusCode is not OK.";
        Assert.That(async () =>
        {
            await shelteredClient.GetAnimalByIdAsync(id, cancellationTokenSource.Token);
        }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
    }

    [Test]
    public async Task GetAnimalByIdAsync__Should_return_the_found_animal_When_an_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var actual = await shelteredClient.GetAnimalByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.EqualTo(new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat, Sex = AnimalSex.Female }));
    }

    [Test]
    public async Task ListAnimalsAsync__Should_return_an_empty_list_of_animals_When_there_are_no_animals_in_the_database()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var actual = await shelteredClient.ListAnimalsAsync(cancellationTokenSource.Token);

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public async Task ListAnimalsAsync__Should_return_a_list_containing_all_the_animals_When_the_database_contains_some_animals()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        Guid lucysId = Guid.Empty;
        Guid jakesId = Guid.Empty;
        Guid neekosId = Guid.Empty;
        await using (var shelteredContext = GetDbContext<ShelteredContext>())
        {
            var lucyTheCat = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            var jakeTheDog = new AnimalEntity
            {
                Name = "Jake",
                Kind = AnimalKind.Dog,
                Sex = AnimalSex.Female
            };

            var neekoTheCat = new AnimalEntity
            {
                Name = "Neeko",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            await shelteredContext.AddRangeAsync([lucyTheCat, jakeTheDog, neekoTheCat], cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);

            lucysId = lucyTheCat.Id;
            jakesId = jakeTheDog.Id;
            neekosId = neekoTheCat.Id;
        }

        var actual = await shelteredClient.ListAnimalsAsync(cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Has.Exactly(3).Items);
            Assert.That(actual, Has.Exactly(1).Matches<KeyValuePair<Guid, AnimalModel>>(idToAnimal =>
            {
                return idToAnimal.Key == lucysId
                    && string.Equals(idToAnimal.Value.Name, "Lucy", StringComparison.InvariantCulture)
                    && idToAnimal.Value.Kind == AnimalKind.Cat;
            }));
            Assert.That(actual, Has.Exactly(1).Matches<KeyValuePair<Guid, AnimalModel>>(idToAnimal =>
            {
                return idToAnimal.Key == jakesId
                    && string.Equals(idToAnimal.Value.Name, "Jake", StringComparison.InvariantCulture)
                    && idToAnimal.Value.Kind == AnimalKind.Dog;
            }));
            Assert.That(actual, Has.Exactly(1).Matches<KeyValuePair<Guid, AnimalModel>>(idToAnimal =>
            {
                return idToAnimal.Key == neekosId
                    && string.Equals(idToAnimal.Value.Name, "Neeko", StringComparison.InvariantCulture)
                    && idToAnimal.Value.Kind == AnimalKind.Cat;
            }));
        });
    }

    [Test]
    public async Task UpdateAnimalByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        var id = Guid.NewGuid();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = GetDbContext<ShelteredContext>();
            Assert.That(actual, Is.False);
            Assert.That(shelteredContext.Animals, Is.Empty);
        });
    }

    [Test]
    public async Task UpdateAnimalByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat,
                Sex = AnimalSex.Female
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var animalModel = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog,
            Sex = AnimalSex.Female
        };

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = GetDbContext<ShelteredContext>();
            Assert.That(actual, Is.True);
            Assert.That(shelteredContext.Animals, Has.Exactly(1).Items);
            Assert.That(shelteredContext.Animals, Has.Exactly(1).Matches<AnimalEntity>(animalEntity =>
            {
                return animalEntity.Id == id
                    && string.Equals(animalEntity.Name, animalModel.Name, StringComparison.InvariantCulture)
                    && animalEntity.Kind == animalModel.Kind;
            }));
        });
    }
}
