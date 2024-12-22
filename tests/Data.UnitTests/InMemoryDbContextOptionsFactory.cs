using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class InMemoryDbContextOptionsFactory<TContext>(string databaseName) : IDbContextOptionsFactory<TContext> where TContext : DbContext
{
    public InMemoryDbContextOptionsFactory() : this($"{nameof(TContext)}InMemoryTestDatabase") { }

    public DbContextOptions<TContext> CreateDbContextOptions()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
        dbContextOptionsBuilder.UseInMemoryDatabase(databaseName);
        return dbContextOptionsBuilder.Options;
    }
}
