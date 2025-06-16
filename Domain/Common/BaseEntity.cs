using Domain.Abstractions;

namespace Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; protected set; }
    }
}
