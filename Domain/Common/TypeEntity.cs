using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Common
{
    public class TypeEntity : BaseEntity
    {
        protected TypeEntity(Guid id) : base(id) { }
        protected TypeEntity(Guid id, Title title) : base(id)
        {
            Title = title;
        }

        Title Title = null!;
        public static Result<TypeEntity> Create(Result<Title> title)
        {
            if (title.IsFailure) return Result.Failure<TypeEntity>(title);
            return new TypeEntity(Guid.NewGuid(), title.Value);
        }
    }
}
