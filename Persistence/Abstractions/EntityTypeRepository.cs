using Domain.Abstractions;
using Domain.Common;
using Domain.Shared;

namespace Persistence.Abstractions
{
    public abstract class EntityTypeRepository<TypeE, EntityWithT> : TRepository<TypeE>, IEntityTypeRepository<TypeE, EntityWithT> 
        where TypeE : TypeEntity
        where EntityWithT : EntityWithTntity<TypeE>
    {
        protected readonly IEntityWithTntityRepository<TypeE, EntityWithT> _EntityWithTntityRepository;
        protected EntityTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<TypeE, EntityWithT> EntityWithTntityRepository) : base(dbContext)
        {
            _EntityWithTntityRepository = EntityWithTntityRepository;
        }

        public override async Task<Result<TypeE>> RemoveAsync(Result<TypeE> entity, CancellationToken cancellationToken = default) 
        {
            if (entity.IsFailure) return entity;
            var teList = await _EntityWithTntityRepository.GetAllAsync(te => te.TypeId == entity.Value.Id, cancellationToken);
            if (teList.IsFailure) return Result.Failure<TypeE>(teList.Error);
            foreach (var te in teList.Value)
                await _EntityWithTntityRepository.RemoveAsync(te, cancellationToken);
            return await base.RemoveAsync(entity, cancellationToken);
        }
    }
}
