using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinFeatureRepository : TRepository<SkinFeature>, ISkinFeatureRepository
    {
        public SkinFeatureRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
