using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Data.Animals;

namespace Data.UnitTests.Animals;

internal sealed class AnimalEntityEqualityComparer(bool includeId, StringComparison stringComparison) : IEqualityComparer<AnimalEntity>
{
    public AnimalEntityEqualityComparer() : this(true, StringComparison.InvariantCulture) { }

    public AnimalEntityEqualityComparer(StringComparison stringComparison) : this(true, stringComparison) { }

    public AnimalEntityEqualityComparer(bool includeId) : this(includeId, StringComparison.InvariantCulture) { }

    public bool Equals(AnimalEntity? x, AnimalEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is not null && y is not null)
        {
            return (!includeId || (x.Id == y.Id)) && string.Equals(x.Name, y.Name, stringComparison) && x.Kind == y.Kind;
        }

        return x is null && y is null;
    }

    public int GetHashCode([DisallowNull] AnimalEntity obj)
    {
        return HashCode.Combine(obj.Id, obj.Name, obj.Kind);
    }
}
