using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal abstract class RepositoryFixture<TContext>(IDbContextFactory<TContext> dbContextFactory) where TContext : DbContext
{
    protected static readonly CancellationTokenSource CancellationTokenSource = new();

    protected TContext DbContext { get; private set; } = null!;

    protected Repository<TContext> Repository { get; private set; } = null!;

    [SetUp]
    public async Task BeforeEachTestAsync()
    {
        DbContext = await dbContextFactory.CreateDbContextAsync(CancellationTokenSource.Token);
        Repository = new Repository<TContext>(DbContext);
    }

    [TearDown]
    public async Task AfterEachTestAsync()
    {
        if (DbContext is not null)
        {
            await DbContext.DisposeAsync();
        }
    }
}
