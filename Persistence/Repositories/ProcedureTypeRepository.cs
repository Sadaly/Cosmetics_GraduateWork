using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
	public class ProcedureTypeRepository(AppDbContext dbContext, IEntityWithTypeRepository<ProcedureType, Procedure> EntityWithTypeRepository)
		: EntityTypeRepository<ProcedureType, Procedure>(dbContext, EntityWithTypeRepository), IProcedureTypeRepository
	{
	}
}
