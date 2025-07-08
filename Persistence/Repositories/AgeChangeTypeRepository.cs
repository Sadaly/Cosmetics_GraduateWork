using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class AgeChangeTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<AgeChangeType, AgeChange> EntityWithTypeRepository) 
        : EntityTypeRepository<AgeChangeType, AgeChange>(dbContext, EntityWithTypeRepository), IAgeChangeTypeRepository
    {
    }
}
