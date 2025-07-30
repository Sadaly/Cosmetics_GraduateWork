using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;

namespace Persistence.Abstractions
{
    public abstract class EntityWithTypeRepository<TypeE, EntityWithT>(AppDbContext dbContext) : TRepository<EntityWithT>(dbContext), IEntityWithTypeRepository<TypeE, EntityWithT>
        where TypeE : TypeEntity
        where EntityWithT : EntityWithType<TypeE>
    {
        public override async Task<Result<EntityWithT>> RemoveAsync(Result<EntityWithT> entity, CancellationToken cancellationToken = default)
        {
            if (entity.IsFailure) return entity;
            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}
