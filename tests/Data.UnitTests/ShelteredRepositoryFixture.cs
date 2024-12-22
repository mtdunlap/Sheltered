using System.Threading;
using System.Threading.Tasks;
using Core.Animals;
using Data.Animals;
using Data.UnitTests.Animals;

namespace Data.UnitTests;

[TestFixture]
[TestFixtureSource(nameof(InMemoryDbContextOptionsFactorySource))]
[Parallelizable(ParallelScope.All)]
internal sealed class ShelteredRepositoryFixture(IDbContextOptionsFactory<ShelteredContext> dbContextOptionsFactory) : RepositoryFixture<ShelteredContext>(dbContextOptionsFactory)
{
    protected override ShelteredContext CreateDbContext()
    {
        return new ShelteredContext(Options);
    }

    [Test]
    public async Task AddAnimalAsync__Should_add_an_animal_to_the_sheltered_context()
    {
        var cancellationToken = CancellationTokenSource.Token;
        var dbContext = CreateDbContext();
        var repository = new ShelteredRepository(dbContext);

        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        await repository.AddAnimalAsync(animalEntity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var expected = new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Cat };
        var animalEntityEqualityComparer = new AnimalEntityEqualityComparer(includeId: false);

        Assert.Multiple(() =>
        {
            Assert.That(dbContext.Animals, Is.Not.Null);
            Assert.That(dbContext.Animals, Has.Exactly(1).Items);
            Assert.That(dbContext.Animals, Has.Exactly(1).EqualTo(expected).Using(animalEntityEqualityComparer));
        });
    }
}
