using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class HealthCondTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<HealthCondType, HealthCond> EntityWithTntityRepository) 
        : EntityTypeRepository<HealthCondType, HealthCond>(dbContext, EntityWithTntityRepository), IHealthCondTypeRepository
    {
    }
}
