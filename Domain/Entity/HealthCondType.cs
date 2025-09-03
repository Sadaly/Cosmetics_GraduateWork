using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
	public class HealthCondType : TypeEntity
	{
		private HealthCondType(Guid id) : base(id) { }
		private HealthCondType(Guid id, Title title) : base(id, title)
		{
		}
		public static Result<HealthCondType> Create(Result<Title> title)
		{
			if (title.IsFailure) return Result.Failure<HealthCondType>(title);
			return new HealthCondType(Guid.NewGuid(), title.Value);
		}
	}
}
