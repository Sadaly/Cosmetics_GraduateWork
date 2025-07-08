using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinCareTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<SkinCareType, SkinCare> EntityWithTntityRepository) 
        : EntityTypeRepository<SkinCareType, SkinCare>(dbContext, EntityWithTntityRepository), ISkinCareTypeRepository
    {
    }
}
