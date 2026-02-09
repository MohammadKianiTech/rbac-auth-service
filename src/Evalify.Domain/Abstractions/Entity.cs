namespace Evalify.Domain.Abstractions;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = new();
    protected Entity(Guid id)
    {
        Id = id;
    }
    protected Entity()
    {
    }
    public Guid Id { get; init; }
    public IReadOnlyList<DomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}