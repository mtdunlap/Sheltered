using System;
using System.Diagnostics.CodeAnalysis;

namespace Data;

public abstract record class Entity
{
    public Guid Id
    {
        get;

        [ExcludeFromCodeCoverage(Justification = "Cannot test a protected initter.")]
        protected init;
    } = Guid.Empty;
}
