using System;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class InMemoryDbContextFactory<TContext>(string databaseName) : IDbContextFactory<TContext> where TContext : DbContext
{
    public TContext CreateDbContext()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.UseInMemoryDatabase(databaseName);
        var dbContextOptions = dbContextOptionsBuilder.Options;
        var context = (TContext?)Activator.CreateInstance(typeof(TContext), [dbContextOptions]);
        return context ?? throw new InvalidOperationException();
    }
}
