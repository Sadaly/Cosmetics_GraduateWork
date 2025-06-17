using Domain.Abstractions;

namespace Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity() { }
        protected BaseEntity(Guid id) { Id = id; }
        public Guid Id { get; protected set; }
    }
}
