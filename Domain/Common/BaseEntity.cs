using Domain.Abstractions;
using Domain.Shared;

namespace Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity() { }
        protected BaseEntity(Guid id) { Id = id; }
        public Guid Id { get; protected set; }
        //public bool IsSoftDelete { get; protected set; }
        //public Date CreatedAt { get; protected set; }

        public virtual Result Delete()
        {
            //IsSoftDelete = true;

            return Result.Success();
        }
    }
}
