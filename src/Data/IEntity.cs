namespace Data;

/// <summary>
/// Represents an entity with a sentinel value when no entity is found.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IEntity<TSelf> where TSelf : Entity
{
    /// <summary>
    /// A sentinel value for when no <typeparamref name="TSelf"/> is found.
    /// </summary>
    static abstract TSelf None { get; }
}
