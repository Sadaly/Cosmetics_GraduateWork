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
		public Result<TypeEntity> Update(Result<Title> title)
		{
			if (title.IsFailure) return title.Error;
			Title = title.Value;
			return this;
		}
		public Title Title { get; set; } = null!;
	}
}
