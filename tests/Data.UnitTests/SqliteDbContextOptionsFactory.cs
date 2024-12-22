using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class SqliteDbContextOptionsFactory<TContext>(string dataSource) : IDbContextOptionsFactory<TContext> where TContext : DbContext
{
    public SqliteDbContextOptionsFactory() : this(":memory:") { }

    public DbContextOptions<TContext> CreateDbContextOptions()
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = dataSource,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
        dbContextOptionsBuilder.UseSqlite(connectionString);
        return dbContextOptionsBuilder.Options;
    }
}
