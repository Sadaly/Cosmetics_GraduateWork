using Domain.Common;

namespace Domain.Abstractions
{
    public interface IEntityTypeRepository<TypeE, TransitiveE> : IRepository<TypeE>
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>
    {
    }
}
