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
        public DateTime UpdateAt { get; protected set; }
        public virtual Result SoftDelete()
        {
            IsSoftDelete = true;
            return Result.Success();
        }
    }
}
