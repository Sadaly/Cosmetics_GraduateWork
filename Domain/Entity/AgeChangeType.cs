using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class AgeChangeType : TypeEntity
    {
        private AgeChangeType(Guid id) : base(id) { }
        private AgeChangeType(Guid id, Title title) : base(id, title)
        {
        }
        public static Result<AgeChangeType> Create(Result<Title> title)
        {
            if (title.IsFailure) return Result.Failure<AgeChangeType>(title);
            return new AgeChangeType(Guid.NewGuid(), title.Value);
        }
    }
}
