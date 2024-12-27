using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal interface IDbContextOptionsFactory
{
    DbContextOptions Create();
}
