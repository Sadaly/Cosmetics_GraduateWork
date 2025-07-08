using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ProcedureTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<ProcedureType, Procedure> EntityWithTntityRepository) 
        : EntityTypeRepository<ProcedureType, Procedure>(dbContext, EntityWithTntityRepository), IProcedureTypeRepository
    {
    }
}
