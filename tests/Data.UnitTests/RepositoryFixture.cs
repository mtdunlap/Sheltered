using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

[NonParallelizable]
internal abstract class RepositoryFixture<TContext>(IDbContextOptionsFactory dbContextOptionsFactory, CancellationTokenSource cancellationTokenSource) where TContext : DbContext
{
    ~RepositoryFixture()
    {
        cancellationTokenSource.Dispose();
    }

    [SetUp]
    protected virtual async Task BeforeEachTestAsync()
    {
        var dbContextOptions = dbContextOptionsFactory.Create();
        Context = CreateContext(dbContextOptions);
        await Context.Database.EnsureCreatedAsync();
    }

    [TearDown]
    protected virtual async Task AfterEachTestAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }

    protected CancellationToken CancellationToken { get; } = cancellationTokenSource.Token;

    protected TContext Context { get; private set; } = null!;

    protected abstract TContext CreateContext(DbContextOptions dbContextOptions);
}
