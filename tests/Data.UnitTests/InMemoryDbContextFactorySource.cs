using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.UnitTests;

internal sealed class InMemoryDbContextFactorySource<TContext> : IEnumerable<TestFixtureData> where TContext : DbContext
{
    public IEnumerator<TestFixtureData> GetEnumerator()
    {
        yield return new TestFixtureData(new InMemoryDbContextFactory<TContext>("InMemoryContext"));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
