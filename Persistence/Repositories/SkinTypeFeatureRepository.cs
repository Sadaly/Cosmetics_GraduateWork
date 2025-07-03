using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinTypeFeatureRepository : EntityTypeRepository<SkinFeatureType, SkinFeature>, ISkinFeatureTypeRepository
    {
        public SkinTypeFeatureRepository(AppDbContext dbContext, ITransitiveEntityRepository<SkinFeatureType, SkinFeature> transitiveEntityRepository) : base(dbContext, transitiveEntityRepository)
        {
        }
    }
}
