using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class SqliteDbContextOptionsFactory(string dataSource, SqliteOpenMode sqliteOpenMode, SqliteCacheMode sqliteCacheMode) : IDbContextOptionsFactory
{
    public SqliteDbContextOptionsFactory(string dataSource, SqliteOpenMode sqliteOpenMode) : this(dataSource, sqliteOpenMode, SqliteCacheMode.Private) { }

    public SqliteDbContextOptionsFactory(string dataSource) : this(dataSource, SqliteOpenMode.ReadWriteCreate, SqliteCacheMode.Private) { }

    public DbContextOptions Create()
    {
        var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = dataSource,
            Mode = sqliteOpenMode,
            Cache = sqliteCacheMode
        };
        var connectionString = sqliteConnectionStringBuilder.ToString();
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.UseSqlite(connectionString);
        return dbContextOptionsBuilder.Options;
    }
}
