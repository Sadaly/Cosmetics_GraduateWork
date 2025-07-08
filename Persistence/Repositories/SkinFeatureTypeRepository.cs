using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinFeatureTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<SkinFeatureType, SkinFeature> EntityWithTntityRepository) 
        : EntityTypeRepository<SkinFeatureType, SkinFeature>(dbContext, EntityWithTntityRepository), ISkinFeatureTypeRepository
    {
    }
}
