using Domain.Common;

namespace Domain.Abstractions
{
    public interface ITransitiveEntityRepository<TypeE, TransitiveE> : IRepository<TransitiveE>
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>;
}
