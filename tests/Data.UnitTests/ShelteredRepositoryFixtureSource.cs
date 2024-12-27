using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Data.UnitTests;

internal sealed class ShelteredRepositoryFixtureSource : IEnumerable<TestFixtureData>
{
    public IEnumerator<TestFixtureData> GetEnumerator()
    {
        yield return new TestFixtureData(InMemoryDbContextOptionsFactory.RandomDatabaseName, new CancellationTokenSource());
        yield return new TestFixtureData(new SqliteDbContextOptionsFactory("../../../sheltered.unittests.db"), new CancellationTokenSource());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
