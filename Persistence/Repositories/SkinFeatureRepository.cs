using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinFeatureRepository : TransitiveEntityRepository<SkinFeatureType, SkinFeature>, ISkinFeatureRepository
    {
        public SkinFeatureRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
