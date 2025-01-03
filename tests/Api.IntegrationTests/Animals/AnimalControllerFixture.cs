using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Client.Animals;
using Core.Animals;
using Data;
using Data.Animals;
using System.Net.Http;

namespace Api.IntegrationTests.Animals;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal sealed class AnimalControllerFixture
{
    private static IEnumerable<TestCaseData> ShelteredContextDatabaseConfigurerSource()
    {
        yield return new TestCaseData(SqliteIntegrationTestDatabaseConfigurer<ShelteredContext>.RandomlyNamedInMemoryConfigurer);
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task AnimalExistsByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var id = Guid.NewGuid();

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.False);
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task AnimalExistsByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task CreateAnimalAsync__Should_insert_a_new_animal_into_the_sheltered_database(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };

        var (created, id) = await shelteredClient.CreateAnimalAsync(animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>();
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
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task DeleteAnimalByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var id = Guid.NewGuid();

        var actual = await shelteredClient.DeleteAnimalByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.False);
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task DeleteAnimalByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat
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
                await using var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>();
                return await shelteredContext.FindAsync<AnimalEntity>([id], cancellationTokenSource.Token);
            }, Is.Null);
        });
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task GetAnimalByIdAsync__Should_throw_an_http_request_exception_When_no_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var id = Guid.NewGuid();

        const string expectedMessage = "statusCode is not OK.";
        Assert.That(async () =>
        {
            await shelteredClient.GetAnimalByIdAsync(id, cancellationTokenSource.Token);
        }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task GetAnimalByIdAsync__Should_return_the_found_animal_When_an_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var actual = await shelteredClient.GetAnimalByIdAsync(id, cancellationTokenSource.Token);

        Assert.That(actual, Is.EqualTo(new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat }));
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task ListAnimalsAsync__Should_return_an_empty_list_of_animals_When_there_are_no_animals_in_the_database(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var actual = await shelteredClient.ListAnimalsAsync(cancellationTokenSource.Token);

        Assert.That(actual, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task ListAnimalsAsync__Should_return_a_list_containing_all_the_animals_When_the_database_contains_some_animals(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        await using (var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>())
        {
            var lucyTheCat = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat
            };

            var jakeTheDog = new AnimalEntity
            {
                Name = "Jake",
                Kind = AnimalKind.Dog
            };

            var neekoTheCat = new AnimalEntity
            {
                Name = "Neeko",
                Kind = AnimalKind.Cat
            };

            await shelteredContext.AddRangeAsync([lucyTheCat, jakeTheDog, neekoTheCat], cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
        }

        var actual = await shelteredClient.ListAnimalsAsync(cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Has.Exactly(3).Items);
            Assert.That(actual, Has.Exactly(1).Matches<AnimalModel>(animalModel =>
            {
                return string.Equals(animalModel.Name, "Lucy", StringComparison.InvariantCulture)
                    && animalModel.Kind == AnimalKind.Cat;
            }));
            Assert.That(actual, Has.Exactly(1).Matches<AnimalModel>(animalModel =>
            {
                return string.Equals(animalModel.Name, "Jake", StringComparison.InvariantCulture)
                    && animalModel.Kind == AnimalKind.Dog;
            }));
            Assert.That(actual, Has.Exactly(1).Matches<AnimalModel>(animalModel =>
            {
                return string.Equals(animalModel.Name, "Neeko", StringComparison.InvariantCulture)
                    && animalModel.Kind == AnimalKind.Cat;
            }));
        });
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task UpdateAnimalByIdAsync__Should_return_false_When_no_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        var id = Guid.NewGuid();

        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>();
            Assert.That(actual, Is.False);
            Assert.That(shelteredContext.Animals, Is.Empty);
        });
    }

    [Test]
    [TestCaseSource(nameof(ShelteredContextDatabaseConfigurerSource))]
    public async Task UpdateAnimalByIdAsync__Should_return_true_When_an_animal_with_the_provided_id_exists(IIntegrationTestDatabaseConfigurer databaseConfigurer)
    {
        await using var webApplicationFactory = new IntegrationTestWebApplicationFactory<Program>(databaseConfigurer);
        using var cancellationTokenSource = new CancellationTokenSource();
        using var shelteredClient = webApplicationFactory.CreateShelteredClient();

        Guid id = Guid.Empty;

        await using (var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>())
        {
            var animalEntity = new AnimalEntity
            {
                Name = "Lucy",
                Kind = AnimalKind.Cat
            };

            _ = await shelteredContext.AddAsync(animalEntity, cancellationTokenSource.Token);
            _ = await shelteredContext.SaveChangesAsync(cancellationTokenSource.Token);
            id = animalEntity.Id;
        }

        var animalModel = new AnimalModel
        {
            Name = "Jake",
            Kind = AnimalKind.Dog
        };

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, cancellationTokenSource.Token);

        Assert.Multiple(async () =>
        {
            await using var shelteredContext = webApplicationFactory.GetDbContext<ShelteredContext>();
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