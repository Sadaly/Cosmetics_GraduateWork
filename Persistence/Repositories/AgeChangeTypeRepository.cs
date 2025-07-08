using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class AgeChangeTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<AgeChangeType, AgeChange> EntityWithTntityRepository) 
        : EntityTypeRepository<AgeChangeType, AgeChange>(dbContext, EntityWithTntityRepository), IAgeChangeTypeRepository
    {
    }
}
