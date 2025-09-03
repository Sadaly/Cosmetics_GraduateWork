using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
	public class SkinFeatureType : TypeEntity
	{
		private SkinFeatureType(Guid id) : base(id) { }
		private SkinFeatureType(Guid id, Title title) : base(id, title)
		{
		}
		public static Result<SkinFeatureType> Create(Result<Title> title)
		{
			if (title.IsFailure) return Result.Failure<SkinFeatureType>(title);
			return new SkinFeatureType(Guid.NewGuid(), title.Value);
		}
	}
}
