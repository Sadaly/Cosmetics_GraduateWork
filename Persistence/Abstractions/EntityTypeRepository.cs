using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;

namespace Persistence.Abstractions
{
    public abstract class EntityTypeRepository<TypeE, TransitiveE> : TRepository<TypeE>, IEntityTypeRepository<TypeE, TransitiveE> 
        where TypeE : TypeEntity
        where TransitiveE : TransitiveEntity<TypeE>
    {
        protected readonly ITransitiveEntityRepository<TypeE, TransitiveE> _transitiveEntityRepository;
        protected EntityTypeRepository(AppDbContext dbContext, ITransitiveEntityRepository<TypeE, TransitiveE> transitiveEntityRepository) : base(dbContext)
        {
            _transitiveEntityRepository = transitiveEntityRepository;
        }

        public override async Task<Result<TypeE>> RemoveAsync(Result<TypeE> entity, CancellationToken cancellationToken = default) 
        {
            if (entity.IsFailure) return entity;
            var teList = await _transitiveEntityRepository.GetAllAsync(te => te.TypeId == entity.Value.Id, cancellationToken);
            if (teList.IsFailure) return Result.Failure<TypeE>(teList.Error);
            foreach (var te in teList.Value)
                await _transitiveEntityRepository.RemoveAsync(te, cancellationToken);
            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}
