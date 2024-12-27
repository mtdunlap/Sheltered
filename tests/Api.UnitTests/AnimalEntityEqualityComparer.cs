using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Data.Animals;

namespace Api.UnitTests;

internal sealed class AnimalEntityEqualityComparer(bool includeId, bool includeCreated, bool includeLastUpdated, StringComparison stringComparison)
    : EntityEqualityComparer<AnimalEntity>(includeId, includeCreated, includeLastUpdated), IEqualityComparer<AnimalEntity>
{
    public AnimalEntityEqualityComparer() : this(false, false, false, StringComparison.InvariantCulture) { }
    public AnimalEntityEqualityComparer(bool includeId) : this(includeId, false, false, StringComparison.InvariantCulture) { }
    public AnimalEntityEqualityComparer(bool includeId, bool includeCreated) : this(includeId, includeCreated, false, StringComparison.InvariantCulture) { }
    public AnimalEntityEqualityComparer(bool includeId, bool includeCreated, bool includeLastUpdated) : this(includeId, includeCreated, includeLastUpdated, StringComparison.InvariantCulture) { }

    public override bool Equals(AnimalEntity? x, AnimalEntity? y)
    {
        var baseEquals = base.Equals(x, y);

        if (x is not null && y is not null)
        {
            return baseEquals && string.Equals(x.Name, y.Name, stringComparison) && x.Kind == y.Kind;
        }

        return x is null && y is null;
    }

    public override int GetHashCode([DisallowNull] AnimalEntity obj)
    {
        return HashCode.Combine(obj.Name, obj.Kind);
    }
}
