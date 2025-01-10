using System;

namespace Data;

public interface IEntity<TSelf> where TSelf : IEntity<TSelf>
{
    static abstract TSelf NotFound { get; }
    Guid Id { get; }
}
