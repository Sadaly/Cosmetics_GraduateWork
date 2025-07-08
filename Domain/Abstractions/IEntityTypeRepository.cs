using Domain.Common;

namespace Domain.Abstractions
{
    public interface IEntityTypeRepository<TypeE, EntityWithT> : IRepository<TypeE>
        where TypeE : TypeEntity
        where EntityWithT : EntityWithType<TypeE>
    {
    }
}
