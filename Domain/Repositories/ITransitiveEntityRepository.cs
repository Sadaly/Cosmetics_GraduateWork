using Domain.Abstractions;
using Domain.Common;

namespace Domain.Repositories
{
    public interface ITransitiveEntityRepository<TypeE, TransitiveE> : IRepository<TransitiveE>
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>;
}
