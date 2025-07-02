using Domain.Abstractions;

public sealed record EntityTypeDeletedDomainEvent(Guid EntityTypeId) : IDomainEvent;
