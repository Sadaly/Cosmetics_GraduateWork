using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinFeatureTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<SkinFeatureType, SkinFeature> EntityWithTypeRepository) 
        : EntityTypeRepository<SkinFeatureType, SkinFeature>(dbContext, EntityWithTypeRepository), ISkinFeatureTypeRepository
    {
    }
}
