using System.Collections;
using System.Collections.Generic;
using Core.Animals;
using Data.Animals;

namespace Data.UnitTests;

internal sealed class ShelteredRepositoryFixtureTestDataSource : IEnumerable<TestCaseData>
{
    public IEnumerator<TestCaseData> GetEnumerator()
    {
        yield return new TestCaseData(new AnimalEntity { Name = "Lucy", Kind = AnimalKind.Cat });
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
