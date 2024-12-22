namespace Data;

public interface IEntity<TSelf> where TSelf : Entity
{
    static abstract TSelf None { get; }
}
