using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Common
{
    public abstract class TypeEntity : BaseEntity
    {
        protected TypeEntity(Guid id) : base(id) { }
        protected TypeEntity(Guid id, Title title) : base(id)
        {
            Title = title;
        }

        public Title Title { get; set; } = null!;
    }
}
