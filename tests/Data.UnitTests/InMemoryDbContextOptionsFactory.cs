using System;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class InMemoryDbContextOptionsFactory(string databaseName) : IDbContextOptionsFactory
{
    public static InMemoryDbContextOptionsFactory RandomDatabaseName
    {
        get
        {
            var guid = Guid.NewGuid();
            var databaseName = guid.ToString();
            return new(databaseName);
        }
    }

    public DbContextOptions Create()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.UseInMemoryDatabase(databaseName);
        return dbContextOptionsBuilder.Options;
    }
}
