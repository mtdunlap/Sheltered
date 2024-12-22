using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal abstract class RepositoryFixture<TContext>(IDbContextOptionsFactory<TContext> dbContextOptionsFactory) where TContext : DbContext
{
    protected static readonly CancellationTokenSource CancellationTokenSource = new();

    protected static IEnumerable<TestFixtureData> InMemoryDbContextOptionsFactorySource()
    {
        yield return new TestFixtureData(new InMemoryDbContextOptionsFactory<ShelteredContext>());
    }

    protected static IEnumerable<TestFixtureData> SqliteDbContextOptionsFactorySource()
    {
        yield return new TestFixtureData(new SqliteDbContextOptionsFactory<ShelteredContext>());
    }

    protected DbContextOptions<TContext> Options => dbContextOptionsFactory.CreateDbContextOptions();

    protected abstract ShelteredContext CreateDbContext();
}
