using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Animals;

namespace Data.UnitTests;

[TestFixture]
[TestFixtureSource(typeof(InMemoryDbContextFactorySource<ShelteredContext>))]
internal sealed class ShelteredRepositoryFixture(IDbContextFactory<ShelteredContext> dbContextFactory) : RepositoryFixture<ShelteredContext>(dbContextFactory)
{
    [Test]
    [TestCaseSource(typeof(ShelteredRepositoryFixtureTestDataSource))]
    public async Task AddAsync__Should<TEntity>(TEntity entity) where TEntity : Entity, IEntity<TEntity>
    {
        await Repository.AddAsync(entity, CancellationTokenSource.Token);
        await Repository.SaveChangesAsync();

        Assert.Multiple(() =>
        {
            Assert.That(DbContext.Set<AnimalEntity>(), Has.One.Items);
            Assert.That(DbContext.Set<AnimalEntity>(), Has.One.EqualTo(entity));
        });
    }
}
