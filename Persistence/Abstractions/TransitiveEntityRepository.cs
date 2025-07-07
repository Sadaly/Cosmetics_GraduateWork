using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;

namespace Persistence.Abstractions
{
    public abstract class TransitiveEntityRepository<TypeE, TransitiveE>(AppDbContext dbContext) : TRepository<TransitiveE>(dbContext), ITransitiveEntityRepository<TypeE, TransitiveE> 
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>
    {
        public override async Task<Result<TransitiveE>> RemoveAsync(Result<TransitiveE> entity, CancellationToken cancellationToken = default) 
        {
            if (entity.IsFailure) return entity;
            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}
