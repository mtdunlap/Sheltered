using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

public interface IDbContextOptionsFactory<TContext> where TContext : DbContext
{
    DbContextOptions<TContext> CreateDbContextOptions();
}
