using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
	public class SkinCareType : TypeEntity
	{
		private SkinCareType(Guid id) : base(id) { }
		private SkinCareType(Guid id, Title title) : base(id, title)
		{
		}
		public static Result<SkinCareType> Create(Result<Title> title)
		{
			if (title.IsFailure) return title.Error;
			return new SkinCareType(Guid.NewGuid(), title.Value);
		}
	}
}
