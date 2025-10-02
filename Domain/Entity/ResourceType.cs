using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
	public class ResourceType : TypeEntity
	{
		public ResourceType(Guid id) : base(id)
		{
		}
		private ResourceType(Guid id, Title title) : base(id, title)
		{
		}
		public List<ProcedureTypeResourceType> ProcedureTypes { get; } = [];
		public static Result<ResourceType> Create(Result<Title> title)
		{
			if (title.IsFailure) return title.Error;
			return new ResourceType(Guid.NewGuid(), title.Value);
		}
	}
}
