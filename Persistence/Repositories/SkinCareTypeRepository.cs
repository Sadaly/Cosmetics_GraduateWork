using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class SkinCareTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<SkinCareType, SkinCare> EntityWithTypeRepository)
        : EntityTypeRepository<SkinCareType, SkinCare>(dbContext, EntityWithTypeRepository), ISkinCareTypeRepository
    {
    }
}
