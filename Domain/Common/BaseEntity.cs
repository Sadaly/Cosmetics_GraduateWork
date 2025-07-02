using Domain.Abstractions;
using Domain.Shared;

namespace Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity() { }
        protected BaseEntity(Guid id) { Id = id; CreatedAt = DateTime.UtcNow; }
        public Guid Id { get; protected set; }
        public bool IsSoftDelete { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
        public void ClearDomainEvents() => _domainEvents.Clear();
        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public virtual Result SoftDelete()
        {
            IsSoftDelete = true;
            return Result.Success();
        }
    }
}
