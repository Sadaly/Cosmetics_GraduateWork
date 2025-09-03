using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
	public class HealthCondTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<HealthCondType, HealthCond> EntityWithTypeRepository)
		: EntityTypeRepository<HealthCondType, HealthCond>(dbContext, EntityWithTypeRepository), IHealthCondTypeRepository
	{
	}
}
