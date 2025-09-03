using Domain.Common;

namespace Domain.Abstractions
{
	public interface IEntityWithTypeRepository<TypeE, EntityWithT> : IRepository<EntityWithT>
		where TypeE : TypeEntity
		where EntityWithT : EntityWithType<TypeE>;
}
