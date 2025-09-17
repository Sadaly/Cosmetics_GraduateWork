using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Common
{
	public class EntityWithType<T> : BaseEntity where T : TypeEntity
	{
		public EntityWithType(Guid id) : base(id) { }
		public EntityWithType(Guid id, T type) : base(id)
		{
			TypeId = type.Id;
			Type = type;
		}

		public Result<EntityWithType<T>> ChangeType(Result<T> type)
		{
			if (type.IsFailure) return type.Error;
			TypeId = type.Value.Id;
			Type = type.Value;
			return this;
		}

		public Guid TypeId { get; set; }
		[JsonIgnore]
		public T Type { get; set; } = null!;
	}
}
