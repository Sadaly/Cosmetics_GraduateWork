using Domain.Common;

namespace Domain.Abstractions
{
    public interface IEntityWithTntityRepository<TypeE, EntityWithT> : IRepository<EntityWithT>
        where TypeE : TypeEntity
        where EntityWithT : EntityWithTntity<TypeE>;
}
