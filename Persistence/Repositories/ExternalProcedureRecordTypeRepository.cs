using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ExternalProcedureRecordTypeRepository(AppDbContext dbContext, IEntityWithTntityRepository<ExternalProcedureRecordType, ExternalProcedureRecord> EntityWithTntityRepository) 
        : EntityTypeRepository<ExternalProcedureRecordType, ExternalProcedureRecord>(dbContext, EntityWithTntityRepository), IExternalProcedureRecordTypeRepository
    {
    }
}
