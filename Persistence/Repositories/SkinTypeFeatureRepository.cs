using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinTypeFeatureRepository : TRepository<SkinFeatureType>, ISkinTypeFeatureRepository
    {
        public SkinTypeFeatureRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
