using Domain.Abstractions;
using Domain.Common;

namespace Domain.Repositories
{
    public interface IEntityTypeRepository<TypeE, TransitiveE> : IRepository<TypeE>
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>
    {
    }
}
